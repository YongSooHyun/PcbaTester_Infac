using CasRnf32NET;
using DbcParserLib;
using EOL_GND.Common;
using EOL_GND.Model;
using InfoBox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.IO.Compression;
using System.IO.Ports;
using System.Linq;
using System.Media;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using TestFramework.Common.StatusLogging;
using TestFramework.PluginTestCell;
using TestFramework.PluginTestCell.TestResults;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 테스트 실행 과정에 발생하는 로그 이벤트 정보를 담고 있다.
    /// </summary>
    class Eloz1TestLogEventArgs : EventArgs
    {
        internal string Message { get; }

        internal Eloz1TestLogEventArgs(string message)
        {
            Message = message;
        }
    }

    /// <summary>
    /// <see cref="MainForm"/>에서 사용하는 ViewModel 클래스.
    /// </summary>
    class MainViewModel
    {
        /// <summary>
        /// 읽은 바코드를 저장하는 글로벌 변수 이름.
        /// </summary>
        internal const string BarcodeVarName = "IPT_PcbBarcode";

        /// <summary>
        /// 1번 보드가 아닌 다른 보드들에서 ISP(다운로드)가 실패할 때 다시 테스트할지 여부.
        /// </summary>
        internal const string IspRetestVarName = "IPT_ISP_Retest";

        /// <summary>
        /// Nova 관련 정보를 저장하는 파일.
        /// </summary>
        internal const string NovaTempFilePath = "D:\\ElozPlugin\\ipts_nova_grp_files.temp";

        /// <summary>
        /// JTAG 관련 정보를 저장하는 파일.
        /// </summary>
        internal const string JtagTempFilePath = "D:\\ElozPlugin\\jtag_project.temp";

        /// <summary>
        /// 상단 픽스처 식별 문자.
        /// </summary>
        internal const char TopFidClassifier = 'T';

        /// <summary>
        /// 하단 픽스처 식별 문자.
        /// </summary>
        internal const char BottomFidClassifier = 'B';

        /// <summary>
        /// 마스크 식별 문자.
        /// </summary>
        internal const char MaskFidClassifier = 'M';

        /// <summary>
        /// eloZ1 테스트 환경 에러 발생 이벤트.
        /// </summary>
        internal event EventHandler<ErrorOccurredEventArgs> Eloz1EnvironmentErrorOccurred;

        /// <summary>
        /// eloZ1 테스트 로그 발생 이벤트.
        /// </summary>
        internal event EventHandler<Eloz1TestLogEventArgs> Eloz1TestLog;

        /// <summary>
        /// eloZ1 테스트 종료시 발생하는 이벤트.
        /// </summary>
        internal event EventHandler<TestRunFinishedEventArgs> Eloz1TestFinished;

        /// <summary>
        /// eloZ1 실행 상태.
        /// </summary>
        internal bool IsEloz1Running => eloz1Device?.Running ?? false;

        // eloZ1 디바이스 통신 클래스 인스턴스.
        private readonly Eloz1 eloz1Device;

        // PLC 디바이스 통신 클래스 인스턴스.
        private readonly Plc plcDevice = new Plc();

        // DIO 디바이스 통신 클래스 인스턴스.
        private readonly Dio dioDevice = new Dio();
        private readonly DioRead dioReadDevice = new DioRead();
        private readonly DioOut dioOutDevice = new DioOut();

        // Novaflash 디바이스.
        private readonly Novaflash novaDevice = new Novaflash();
        private readonly object novaLockObj = new object();

        // 로깅 스텝 개수를 카운팅하는 변수.
        internal int LoggedSteps { get; set; } = 0;

        // 프린트하려는 문자열.
        private string printingText = "";

        // Power 디바이스.
        private readonly PowerSupply power1Device = new PowerSupply(PowerSupply.Power1Name);
        private readonly PowerSupply power2Device = new PowerSupply(PowerSupply.Power2Name);
        private readonly object power12Lock = new object();

        // 바코드 디바이스.
        private readonly BarcodeScanner barcodeScanner = new BarcodeScanner();

        // 섹션들을 여러번 실행하는 경우, 결과 로그가 클리어되므로 결과들을 보관한다.
        private string prevProjectName = "";
        private readonly Dictionary<int, StringBuilder> projectLogs = new Dictionary<int, StringBuilder>();

        //private readonly Dictionary<int, List<EOL_SNS.Model.TestResultRecordData>> eolResults = new Dictionary<int, List<EOL_SNS.Model.TestResultRecordData>>();
        private readonly Dictionary<int, List<EolStep.TestResult>> eolRunResults = new Dictionary<int, List<EolStep.TestResult>>();
        internal int EolBoardIndex { get; set; } = 0;
        private const int EolIctLogStartIndex = 100;
        private readonly Dictionary<int, List<string>> eolFailMessages = new Dictionary<int, List<string>>();
        private const int EolSpecLogStartIndex = 1000;
        private readonly Dictionary<int, (int RecordCount, long TotalTime)> eolSpecInfo = new Dictionary<int, (int RecordCount, long TotalTime)>();

        // Fixture Probe Count 관리를 위한 변수.
        private static FixtureProbeCountManager probeCountManager;

        // 로그를 꾸미는데 사용되는 문자열.
        private readonly string projUnderline = new string('=', 35);
        private readonly string sectionHeaderMark = "Section:";
        private readonly string sectionUnderline = new string('#', 34);
        private readonly string stepUnderline = new string('-', 34);
        private string currentSectionName = "";

        // Notepad에 저장될 로그.
        private readonly StringBuilder notepadLogBuilder = new StringBuilder();

        // 오디오 플레이에 쓰이는 플레이어.
        //private WindowsMediaPlayer wmPlayer;

        internal MainViewModel()
        {
            eloz1Device = new Eloz1();
            eloz1Device.ErrorOccurred += OnEloz1EnvironmentErrorOccurred;
            eloz1Device.NewTestResult += Eloz1Device_NewTestResult;
            eloz1Device.TestRunFinished += OnEloz1TestFinished;
            EOL_GND.ViewModel.SequenceViewModel.TestFinished += SequenceViewModel_TestFinished;

            probeCountManager = FixtureProbeCountManager.Load();
        }

        private void SequenceViewModel_TestFinished(object sender, List<EolStep.TestResult> testResults)
        {
            // 테스트 결과 저장.
            if (!eolRunResults.ContainsKey(EolBoardIndex))
            {
                eolRunResults.Add(EolBoardIndex, testResults);
            }
            else
            {
                eolRunResults[EolBoardIndex].AddRange(testResults);
            }

            // 로그 텍스트 생성.

            var logBuilder = new StringBuilder();

            // 로깅 스텝 수.
            var maxStepCount = AppSettings.LoggingStepCount;
            var logMode = AppSettings.ElozLogMode;
            
            foreach (var result in testResults)
            {
                result.IctSectionName = currentSectionName;

                // 로깅 스텝 수 계산.
                if (maxStepCount > 0 && LoggedSteps >= maxStepCount)
                {
                    break;
                }

                // 로그 모드에 따라 로깅힌다.
                if (logMode == Project.LogMode.LogDefault && (result.ResultState == EolStep.ResultState.Pass || result.ResultState == EolStep.ResultState.NoState))
                {
                    continue;
                }

                // 테스트 결과를 무시하도록 설정된 로그의 경우는 로깅하지 않는다.
                if (result.Step.IgnoreResult)
                {
                    return;
                }

                string logRecord = CreateEolLogRecord(result);
                if (!string.IsNullOrWhiteSpace(logRecord))
                {
                    LoggedSteps++;
                    logBuilder.Append(logRecord);
                }
            }

            if (logBuilder.Length > 0)
            {
                Eloz1TestLog?.Invoke(this, new Eloz1TestLogEventArgs(logBuilder.ToString()));
            }
        }

        private void SequenceViewModel_TestStepRunFinished(EOL_GND.Model.EolStep.TestResult result)
        {
            List<EolStep.TestResult> testResults;
            if (!eolRunResults.ContainsKey(EolBoardIndex))
            {
                testResults = new List<EolStep.TestResult>();
                eolRunResults.Add(EolBoardIndex, testResults);
            }
            else
            {
                testResults = eolRunResults[EolBoardIndex];
            }

            result.IctSectionName = currentSectionName;
            testResults.Add(result);

            // 로그 텍스트 생성.

            // 로깅 스텝 수를 제한한다.
            var maxStepCount = AppSettings.LoggingStepCount;
            var logMode = AppSettings.ElozLogMode;
            if (maxStepCount > 0 && LoggedSteps >= maxStepCount)
            {
                return;
            }

            // 로그 모드에 따라 로깅힌다.
            if (logMode == Project.LogMode.LogDefault && (result.ResultState == EolStep.ResultState.Pass || result.ResultState == EolStep.ResultState.NoState))
            {
                return;
            }

            // 테스트 결과를 무시하도록 설정된 로그의 경우는 로깅하지 않는다.
            if (result.Step.IgnoreResult)
            {
                return;
            }

            string logRecord = CreateEolLogRecord(result);
            if (!string.IsNullOrWhiteSpace(logRecord))
            {
                LoggedSteps++;
                Eloz1TestLog?.Invoke(this, new Eloz1TestLogEventArgs(logRecord));
            }
        }

        /// <summary>
        /// PLC 디바이스를 오픈한다.
        /// </summary>
        internal void PlcOpen()
        {
            plcDevice.Open();
        }

        internal void DioRWOpen()
        {
            dioReadDevice.Open();
            dioOutDevice.Open();
        }

        /// <summary>
        /// PLC 디바이스를 닫는다.
        /// </summary>
        internal void PlcClose()
        {
            plcDevice.Close();
        }

        internal void DioRWClose()
        {
            dioReadDevice.Close();
            dioOutDevice.Close();
        }

        /// <summary>
        /// PLC 디바이스의 연결 상태를 확인한다.
        /// </summary>
        /// <returns></returns>
        internal bool PlcCheckConnection()
        {
            try
            {
                PlcOpen();
                bool connected = plcDevice.CheckVersion(500) != null;
                return connected;
            }
            catch (Exception e)
            {
                Logger.LogError($"PLC connection error: {e.Message}");
            }
            finally
            {
                PlcClose();
            }

            return false;
        }

        internal bool DioRWCheckConnection()
        {
            try
            {
                DioRWOpen();
                bool dioReadConnected = dioReadDevice.CheckVersion(500) != null;
                bool dioOutConnected = dioOutDevice.CheckVersion(500) != null;
                return dioReadConnected && dioOutConnected;
            }
            catch (Exception e)
            {
                Logger.LogError($"DIO RW connection error: {e.Message}");
            }
            finally
            {
                DioRWClose();
            }

            return false;
        }

        /// <summary>
        /// Power Suppoly 연결 상태를 확인한다.
        /// </summary>
        /// <returns></returns>
        internal bool Power1CheckConnection()
        {
            try
            {
                power1Device.Open();
                return power1Device.CheckConnected(500);
            }
            catch (Exception e)
            {
                Logger.LogError($"{power1Device.DeviceName}: {e.Message}");
            }
            finally
            {
                power1Device.Close();
            }
            return false;
        }

        /// <summary>
        /// Power Suppoly 연결 상태를 확인한다.
        /// </summary>
        /// <returns></returns>
        internal bool Power2CheckConnection()
        {
            try
            {
                power2Device.Open();
                return power2Device.CheckConnected(500);
            }
            catch (Exception e)
            {
                Logger.LogError($"{power2Device.DeviceName}: {e.Message}");
            }
            finally
            {
                power2Device.Close();
            }
            return false;
        }

        /// <summary>
        /// Power1의 전압, 전류를 설정한다.
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="current"></param>
        internal void Power1SetVA(float voltage, float? current)
        {
            try
            {
                power1Device.Open();
                power1Device.SetPower(voltage, current);
            }
            finally
            {
                power1Device.Close();
            }
        }

        /// <summary>
        /// Power1의 전압, 전류를 설정한다.
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="current"></param>
        internal void Power2SetVA(float voltage, float? current)
        {
            try
            {
                power2Device.Open();
                power2Device.SetPower(voltage, current);
            }
            finally
            {
                power2Device.Close();
            }
        }

        /// <summary>
        /// Power1을 초기화한다.
        /// </summary>
        internal void Power1Reset()
        {
            try
            {
                power1Device.Open();
                power1Device.Reset();
            }
            finally
            {
                power1Device.Close();
            }
        }

        /// <summary>
        /// Power2을 초기화한다.
        /// </summary>
        internal void Power2Reset()
        {
            try
            {
                power2Device.Open();
                power2Device.Reset();
            }
            finally
            {
                power2Device.Close();
            }
        }

        /// <summary>
        /// Power1을 On/Off 한다.
        /// </summary>
        /// <param name="on"></param>
        internal void Power1On(bool on)
        {
            try
            {
                power1Device.Open();
                power1Device.SetOutput(on);
            }
            finally
            {
                power1Device.Close();
            }
        }

        /// <summary>
        /// Power2을 On/Off 한다.
        /// </summary>
        /// <param name="on"></param>
        internal void Power2On(bool on)
        {
            try
            {
                power2Device.Open();
                power2Device.SetOutput(on);
            }
            finally
            {
                power2Device.Close();
            }
        }

        internal void Eloz1Open(string projectName)
        {
            if (eloz1Device != null)
            {
                eloz1Device.OpenProject(projectName, false);
            }
        }

        /// <summary>
        /// eloZ1 Test Editor 를 보여준다.
        /// </summary>
        /// <param name="projectName"></param>
        internal void Eloz1ShowTestEditor(string projectName)
        {
            if (eloz1Device != null)
            {
                eloz1Device.ShowTestEditor(projectName);
            }
        }

        /// <summary>
        /// 지정한 eloZ1 프로젝트를 실행한다.
        /// </summary>
        /// <param name="ictProjectName">Attach 된 eloZ1 프로젝트 이름.</param>
        /// <returns></returns>
        internal void Eloz1Run(string sectionName, string variantName)
        {
            if (eloz1Device != null)
            {
                //var fullFileName = Path.Combine(AppSettings.IctFolder, ictFileName);
                eloz1Device.RunTest(sectionName, variantName);
            }
        }

        /// <summary>
        /// 실행되고 있는 eloZ1 프로젝트를 중지한다.
        /// </summary>
        internal void Eloz1Stop()
        {
            eloz1Device?.StopTest();
        }

        /// <summary>
        /// Eloz1 의 테스트가 끝날 때 호출된다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnEloz1TestFinished(object sender, TestRunFinishedEventArgs e)
        {
            Eloz1TestFinished?.Invoke(this, e);
        }

        /// <summary>
        /// Eloz1 에서 새로운 테스트 결과가 생성될 때마다 호출된다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Eloz1Device_NewTestResult(object sender, NewTestResultEventArgs e)
        {
            int maxStepCount = AppSettings.LoggingStepCount;
            var logMode = AppSettings.ElozLogMode;
            StringBuilder logText = new StringBuilder();

            switch (e.ResultRecord.Type)
            {
                case TestResultRecord.RecordType.BeginProgram:
                    string projectName = ((BeginProgramTestResultRecord)e.ResultRecord).ProjectName;
                    if (!projectName.Equals(prevProjectName, StringComparison.OrdinalIgnoreCase))
                    {
                        //logText.AppendLine($"Proj: {projectName}");
                        //logText.AppendLine(projectName);
                        logText.AppendLine(projUnderline);
                    }
                    prevProjectName = projectName;

                    break;
                case TestResultRecord.RecordType.BeginSection:
                    BeginSectionTestResultRecord sectionRecord = (BeginSectionTestResultRecord)e.ResultRecord;
                    string sectionLine = sectionHeaderMark + " " + sectionRecord.SectionName;
                    bool retest = ((BeginSectionTestResultRecord)e.ResultRecord).Retest;
                    if (retest)
                    {
                        sectionLine += " (*** RETEST ***)";
                    }
                    logText.AppendLine(sectionLine);
                    logText.AppendLine(sectionUnderline);
                    LoggedSteps = 0;
                    currentSectionName = sectionRecord.SectionName;
                    break;
                case TestResultRecord.RecordType.TestStep:
                    TestStepResultRecord result = (TestStepResultRecord)e.ResultRecord;
                    //logText.AppendLine($"Step ID={result.ResultID}, State={result.ResultState}, Retest={result.RetestID}");
                    //string indent = "  ";
                    //logText.AppendLine($"Step ID={result.ResultID}, State={result.ResultState}, Items={result.Count}");
                    for (int i = 0; i < result.Count; i++)
                    {
                        // 로깅 스텝 수를 제한한다.
                        if (maxStepCount > 0 && LoggedSteps >= maxStepCount)
                        {
                            break;
                        }

                        var itemResult = result[i];

                        // 로그 모드에 따라 로깅힌다.
                        if (logMode == Project.LogMode.LogDefault && (itemResult.ResultState == ResultState.Pass || itemResult.ResultState == ResultState.NoState))
                        {
                            continue;
                        }

                        logText.Append(CreateIctLogRecord(itemResult));
                        LoggedSteps++;
                    }


                    break;
                case TestResultRecord.RecordType.EndSection:
                    //ResultState = ((EndSectionTestResultRecord)e.ResultRecord).ResultState;
                    logText.AppendLine(sectionUnderline);
                    break;
                case TestResultRecord.RecordType.EndProgram:
                    switch (((EndProgramTestResultRecord)e.ResultRecord).TestState)
                    {
                        case ResultState.Pass:
                            logText.AppendLine("*** PASS *** ");
                            break;
                        case ResultState.Fail:
                            logText.AppendLine("*** FAIL *** ");
                            break;
                        case ResultState.Aborted:
                            logText.AppendLine("*** ABORTED *** ");
                            break;
                        case ResultState.NoState:
                            logText.AppendLine("*** NO TEST *** ");
                            break;
                    }
                    //if (FaultCount > 0)
                    //{
                    //    Text += "\r\n" + "Number of faults: " + FaultCount.ToString() + "\r\n";
                    //}
                    //logText.AppendLine();
                    //logText.AppendLine(Fill("_/\\", 35));
                    // cutting line
                    //logText.AppendLine(projUnderline);
                    break;
            }

            string logMessage = logText.ToString();
            if (!string.IsNullOrEmpty(logMessage))
            {
                Eloz1TestLog?.Invoke(this, new Eloz1TestLogEventArgs(logMessage));
            }
        }

        private StringBuilder CreateIctLogRecord(TestStepResultRecord.ItemTestResultRecord record)
        {
            var logText = new StringBuilder();

            // Step 이름 출력.
            logText.Append("Step  : " + record.TestStepName);

            // 테스트 결과 출력.
            logText.AppendLine(" -> " + record.ResultState.ToString().ToUpper());

            // 단위를 붙여 출력한다.

            // 하한값.
            string lowerLimitText;
            string unit = "";
            double multiplier = 1;
            if (record.ResultValueLowerLimit != null)
            {
                ElozResultLogFormatter.HumanReadableFormat(record.ResultValueLowerLimit ?? 0, record.ResultValueUnit, out MetricPrefix prefix);
                var measurementUnit = PhysicalUnitExtensions.From(record.ResultValueUnit);
                unit = prefix.GetText() + measurementUnit.GetText();
                multiplier = prefix.GetMultiplier();
                lowerLimitText = $"{record.ResultValueLowerLimit / multiplier:0.####}{unit}";
            }
            else
            {
                lowerLimitText = "P";
            }

            // 상한값.
            string upperLimitText;
            if (record.ResultValueUpperLimit != null)
            {
                if (string.IsNullOrEmpty(unit))
                {
                    ElozResultLogFormatter.HumanReadableFormat(record.ResultValueUpperLimit ?? 0, record.ResultValueUnit, out MetricPrefix prefix);
                    var measurementUnit = PhysicalUnitExtensions.From(record.ResultValueUnit);
                    unit = prefix.GetText() + measurementUnit.GetText();
                    multiplier = prefix.GetMultiplier();
                }
                upperLimitText = $"{record.ResultValueUpperLimit / multiplier:0.####}{unit}";
            }
            else
            {
                upperLimitText = "P";
            }

            // 표준값.
            string nominalValueText;
            if (record.ResultNominalValue != null)
            {
                if (string.IsNullOrEmpty(unit))
                {
                    ElozResultLogFormatter.HumanReadableFormat(record.ResultNominalValue ?? 0, record.ResultValueUnit, out MetricPrefix prefix);
                    var measurementUnit = PhysicalUnitExtensions.From(record.ResultValueUnit);
                    unit = prefix.GetText() + measurementUnit.GetText();
                    multiplier = prefix.GetMultiplier();
                }
                nominalValueText = $"{record.ResultNominalValue / multiplier:0.####}{unit}";
            }
            else
            {
                nominalValueText = "NONE";
            }

            // 측정값.
            string valueText;
            if (record.ResultValue != null)
            {
                if (string.IsNullOrEmpty(unit))
                {
                    ElozResultLogFormatter.HumanReadableFormat(record.ResultValue ?? 0, record.ResultValueUnit, out MetricPrefix prefix);
                    var measurementUnit = PhysicalUnitExtensions.From(record.ResultValueUnit);
                    unit = prefix.GetText() + measurementUnit.GetText();
                    multiplier = prefix.GetMultiplier();
                }
                valueText = $"{record.ResultValue / multiplier:0.####}{unit}";
            }
            else
            {
                valueText = "NONE";
            }

            // 표준값 출력.
            logText.AppendLine("StdVal: " + nominalValueText);

            // 하한, 상한 출력.
            logText.AppendLine("LL    : " + lowerLimitText + ", HL: " + upperLimitText);

            // 측정값 출력.
            logText.AppendLine("Meas  : " + valueText);

            // NodeA, NodeB, FixtureNailNumber, ResultInfo.
            bool delimiterNeeded = false;
            if (!string.IsNullOrEmpty(record.NodeAName))
            {
                logText.Append("Node A: " + record.NodeAName);
                delimiterNeeded = true;
            }

            if (!string.IsNullOrEmpty(record.NodeBName))
            {
                if (delimiterNeeded)
                {
                    logText.Append(", ");
                }
                logText.AppendLine("Node B: " + record.NodeBName);
            }
            else
            {
                if (delimiterNeeded)
                {
                    logText.AppendLine();
                }
            }

            delimiterNeeded = false;
            if (record.FixtureNailNumber != null)
            {
                logText.Append("FixNail: " + record.FixtureNailNumber);
                delimiterNeeded = true;
            }

            if (!string.IsNullOrEmpty(record.ResultInfo))
            {
                if (delimiterNeeded)
                {
                    logText.Append(", ");
                }
                logText.AppendLine("ResInfo: " + record.ResultInfo);
            }
            else
            {
                if (delimiterNeeded)
                {
                    logText.AppendLine();
                }
            }

            // TestData TestChannels.
            var testData = eloz1Device.CurrentProject.GetItem("TestData");
            var testStep = testData.GetItem("TestStep", record.TestStepRecordID);

            // Type.
            string templateName = testStep.GetProperty("TemplateName").ToString();
            StringComparison templateComparison = StringComparison.OrdinalIgnoreCase;
            if (templateName.Equals("DischargeTest", templateComparison) ||
                templateName.Equals("ContactTest", templateComparison) ||
                templateName.Equals("ShortsTest", templateComparison) ||
                templateName.Equals("OpensTest", templateComparison))
            {
            }
            else
            {
                var testChannels = testStep.GetItem("TestChannels");
                var all = testChannels.GetProperty("All") as int[];
                var stimulus = testChannels.GetProperty("Stimulus") as int[];
                var guard = testChannels.GetProperty("Guard") as int[];

                if (all != null && all.Length > 0)
                {
                    var arrayStr = string.Join(",", Array.ConvertAll(all, data => data.ToString()));
                    logText.AppendLine("Channel: " + arrayStr);
                }

                delimiterNeeded = false;
                if (guard != null && guard.Length > 0)
                {
                    var arrayStr = string.Join(",", Array.ConvertAll(guard, data => data.ToString()));
                    logText.Append("Guard : " + arrayStr);
                    delimiterNeeded = true;
                }

                if (stimulus != null && stimulus.Length > 0)
                {
                    var arrayStr = string.Join(",", Array.ConvertAll(stimulus, data => data.ToString()));
                    if (delimiterNeeded)
                    {
                        logText.Append(", ");
                    }
                    logText.AppendLine("Stimulus: " + arrayStr);
                }
                else
                {
                    if (delimiterNeeded)
                    {
                        logText.AppendLine();
                    }
                }
            }

            // Split Line 출력.
            logText.AppendLine(stepUnderline);
            return logText;
        }

        private string CreateEolLogRecord(EolStep.TestResult result)
        {
            StringBuilder logText = new StringBuilder();

            // Step 이름 출력.
            logText.Append("Step  : ID=" + result.Step.Id + ", " + result.Step.Name);

            // 테스트 결과 출력.
            logText.AppendLine(" -> " + result.ResultState.ToString().ToUpper());

            // 단위를 붙여 출력한다.

            // 하한값.
            result.Step.GetNominalValues(out double? resultNominalValue, out double? resultValueUpperLimit, out double? resultValueLowerLimit);
            string lowerLimitText;
            string unit = "";
            double multiplier = 1;
            if (resultValueLowerLimit != null)
            {
                EolStep.GetPrefixExpression(resultValueLowerLimit ?? 0, result.Unit, out MetricPrefix prefix);
                unit = prefix.GetText() + result.Unit.GetText();
                multiplier = prefix.GetMultiplier();
                lowerLimitText = $"{resultValueLowerLimit / multiplier:0.####}{unit}";
            }
            else
            {
                lowerLimitText = "P";
            }

            // 상한값.
            string upperLimitText;
            if (resultValueUpperLimit != null)
            {
                if (string.IsNullOrEmpty(unit))
                {
                    EolStep.GetPrefixExpression(resultValueUpperLimit ?? 0, result.Unit, out MetricPrefix prefix);
                    unit = prefix.GetText() + result.Unit.GetText();
                    multiplier = prefix.GetMultiplier();
                }
                upperLimitText = $"{resultValueUpperLimit / multiplier:0.####}{unit}";
            }
            else
            {
                upperLimitText = "P";
            }

            // 표준값.
            string nominalValueText;
            if (resultNominalValue != null)
            {
                if (string.IsNullOrEmpty(unit))
                {
                    EolStep.GetPrefixExpression(resultNominalValue ?? 0, result.Unit, out MetricPrefix prefix);
                    unit = prefix.GetText() + result.Unit.GetText();
                    multiplier = prefix.GetMultiplier();
                }
                nominalValueText = $"{resultNominalValue / multiplier:0.####}{unit}";
            }
            else
            {
                nominalValueText = "NONE";
            }

            // 측정값.
            string valueText;
            if (result.ResultValue != null)
            {
                if (string.IsNullOrEmpty(unit))
                {
                    EolStep.GetPrefixExpression(result.ResultValue ?? 0, result.Unit, out MetricPrefix prefix);
                    unit = prefix.GetText() + result.Unit.GetText();
                    multiplier = prefix.GetMultiplier();
                }
                valueText = $"{result.ResultValue / multiplier:0.####}{unit}";
            }
            else
            {
                valueText = "NONE";
            }

            // 표준값 출력.
            logText.AppendLine("StdVal: " + nominalValueText);

            // 하한, 상한 출력.
            logText.AppendLine("LL    : " + lowerLimitText + ", HL: " + upperLimitText);

            // 측정값 출력.
            logText.AppendLine("Meas  : " + valueText);

            // ResultInfo.
            //if (!string.IsNullOrWhiteSpace(result.ResultInfo))
            //{
            //    logText.AppendLine("ResInfo: " + result.ResultInfo);
            //}

            // Test channels.
            //if (result.Step.AllTestChannels?.Count > 0)
            //{
            //    logText.AppendLine("Channel: " + string.Join(",", result.Step.AllTestChannels));
            //}

            // Split Line 출력.
            logText.AppendLine(stepUnderline);
            return logText.ToString();
        }

        /// <summary>
        /// Repeatedly fills a string with a given fill-string.
        /// </summary>
        /// <param name="Filler"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static string Fill(string Filler, int Length)
        {
            string Text = "";

            while (Length > 0)
            {
                if (Length >= Filler.Length)
                    Text += Filler;
                else
                    Text += Filler.Substring(0, Length);
                Length -= Filler.Length;
            }

            return Text;
        }

        protected void OnEloz1EnvironmentErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            Eloz1EnvironmentErrorOccurred?.Invoke(this, e);
        }

        /// <summary>
        /// ViewModel 에서 사용한 자원들을 해제한다.
        /// </summary>
        internal void Close()
        {
            eloz1Device.ErrorOccurred -= OnEloz1EnvironmentErrorOccurred;
            eloz1Device.NewTestResult -= Eloz1Device_NewTestResult;
            eloz1Device.TestRunFinished -= OnEloz1TestFinished;
            EOL_GND.ViewModel.SequenceViewModel.TestFinished -= SequenceViewModel_TestFinished;

            dioDevice.Close();
            dioReadDevice.Close();
            dioOutDevice.Close();
            plcDevice.Close();
            eloz1Device?.Close();
            novaDevice?.Close();

            if (AppSettings.UseJtag)
            {
                Cascon.Close();
            }
        }

        /// <summary>
        /// 바코드를 읽는다.
        /// 바코드를 읽을 때까지 블록되므로, 별도의 스레드에서 실행해야 한다.
        /// </summary>
        /// <returns></returns>
        internal string ScannerReadBarcode(int readTimeout = 500)
        {
            string barcode = barcodeScanner.ReadBarcode(readTimeout);
            return barcode.Trim();
        }

        /// <summary>
        /// 바코드를 파싱하여 FGCODE를 리턴한다.
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        internal static string ParseBarcode(string barcode)
        {
            // Barcode 파싱.
            string fgCode;
            int commaIndex = barcode.IndexOf(",");
            if (commaIndex >= 0)
            {
                fgCode = barcode.Substring(0, commaIndex);
            }
            else
            {
                fgCode = barcode;
            }
            return fgCode;
        }

        /// <summary>
        /// 바코드를 파싱하여 FID를 추출한다.
        /// </summary>
        /// <param name="barcode">파싱하려는 바코드 문자열.</param>
        /// <returns>FID.</returns>
        internal static int ExtractFid(string barcode, out char classifier)
        {
            // 마지막에서 4 ~ 2번째(3글자)가 FID, 마지막 1번째가 종류이다.
            int length = barcode.Length;
            classifier = barcode[length - 1];
            string fidStr = barcode.Substring(length - 4, 3);
            return int.Parse(fidStr);
        }

        /// <summary>
        /// PLC 디바이스의 상태를 읽어서 리턴한다.
        /// </summary>
        /// <returns>각종 상태를 나타내는 깃발.</returns>
        internal PlcReadFlags PlcReadStatus(bool showLog = true)
        {
            plcDevice.Open();
            return plcDevice.ReadStatus(showLog);
        }

        internal DioReadFlags DioRWReadStatus(bool showLog = true)
        {
            dioReadDevice.Open();
            return dioReadDevice.ReadStatus(showLog, 500);
        }

        internal void PlcSetMode(bool auto)
        {
            plcDevice.SetMode(auto);
        }

        /// <summary>
        /// PLC 디바이스에 실린더 다운 명령을 전송한다.
        /// 실린더가 다 다운된 다음 리턴하므로 시간이 걸릴 수 있다.
        /// </summary>
        internal void PlcCylinderDown()
        {
            plcDevice.CylinderDown();
        }

        internal void DioRWCylinderDown()
        {
            dioOutDevice.CylinderDown();
        }

        /// <summary>
        /// PLC 디바이스에 실린더 상승 명령을 전송한다.
        /// 실린더가 다 올라간 다음 리턴하므로 시간이 걸릴 수 있다.
        /// </summary>
        internal void PlcCylinderUp()
        {
            plcDevice.CylinderUp();
        }

        internal void DioRWCylinderUp()
        {
            dioOutDevice.CylinderUp();
        }

        /// <summary>
        /// PLC 디바이스에 실린더 초기화 명령을 전송한다.
        /// 시간이 걸릴 수 있다.
        /// </summary>
        internal void PlcCylinderInit()
        {
            plcDevice.CylinderInit();
        }

        internal void DioRWCylinderInit()
        {
            dioOutDevice.CylinderInit();
        }

        internal PcbTestResult PlcMovePcb(PcbScanZone scanZone, PcbMesResult mesResult, PcbTestResult testResult)
        {
            plcDevice.Open();
            return plcDevice.MovePcb(scanZone, mesResult, testResult);
        }

        internal void PlcSendError()
        {
            plcDevice.SendError();
        }

        internal void DioOpen()
        {
            dioDevice.Open();
        }

        internal void DioClose()
        {
            dioDevice.Close();
        }

        internal bool DioCheckConnection()
        {
            try
            {
                dioDevice.Open();
                dioDevice.CheckVersion();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"{Dio.DeviceName}: {ex.Message}");
                return false;
            }
        }

        internal void DioFixturePower(bool on, bool showLog)
        {
            if (on)
            {
                dioDevice.FixturePowerOn(showLog);
            }
            else
            {
                dioDevice.FixturePowerOff(showLog);
            }
        }

        internal void DioPowerOff()
        {
            dioDevice.PowerOff();
        }

        internal void DioDischarge(bool on)
        {
            if (on)
            {
                dioDevice.DischargeOn();
            }
            else
            {
                dioDevice.DischargeOff();
            }
        }

        /// <summary>
        /// 제품 정보를 로딩하고, 해당 FGCODE를 가진 제품을 리턴한다.
        /// </summary>
        /// <param name="fgCode">찾으려는 FGCODE.</param>
        /// <returns>해당 FGCODE를 가진 제품 정보가 없으면 null을 리턴한다.</returns>
        internal Product LoadProduct(string fgCode)
        {
            if (string.IsNullOrEmpty(fgCode))
            {
                return null;
            }

            var products = ProductSettingsViewModel.GetProducts();
            foreach (var product in products)
            {
                if (fgCode.Equals(product.FGCode, StringComparison.OrdinalIgnoreCase))
                {
                    return product;
                } 
            }

            return null;
        }

        /// <summary>
        /// 제품 정보를 로딩하고, 해당 프로젝트 이름을 가진 FGCODE를 리턴한다.
        /// </summary>
        /// <param name="projectPath">찾으려는 프로젝트 경로.</param>
        /// <returns>해당 프로젝트 이름을 가진 제품 정보가 없으면 null을 리턴한다.</returns>
        internal Product LoadProductByProject(string projectPath)
        {
            if (string.IsNullOrEmpty(projectPath))
            {
                return null;
            }

            var products = ProductSettingsViewModel.GetProducts();
            foreach (var product in products)
            {
                if (projectPath.Equals(product.ProjectPath, StringComparison.OrdinalIgnoreCase))
                {
                    return product;
                }
            }

            return null;
        }

        /// <summary>
        /// PLC 디바이스에 FID를 전송한다.
        /// </summary>
        internal void PlcSendFid(int fid)
        {
            plcDevice.SendFid(fid);
        }

        internal int DioRWReadFid(bool showLog = true)
        {
            dioReadDevice.Open();
            return dioReadDevice.ReadFid(showLog);
        }

        internal void ClearLog()
        {
            prevProjectName = "";
            projectLogs.Clear();
            eolRunResults.Clear();
            eolFailMessages.Clear();
            eolSpecInfo.Clear();
        }

        internal void RemoveEolLog(int boardIndex)
        {
            eolRunResults.Remove(boardIndex);
            eolFailMessages.Remove(boardIndex);
        }

        internal void CreateLog(int boardIndex, bool isEOL)
        {
            // ICT 로그.
            StringBuilder log;
            int key = boardIndex + (isEOL ? EolIctLogStartIndex : 0);
            bool addHeader;
            if (!projectLogs.ContainsKey(key))
            {
                log = new StringBuilder();
                projectLogs.Add(key, log);
                addHeader = true;
            }
            else
            {
                log = projectLogs[key];
                addHeader = false;
            }

            List<string> failMessages;
            if (eolFailMessages.ContainsKey(boardIndex))
            {
                failMessages = eolFailMessages[boardIndex];
            }
            else
            {
                failMessages = new List<string>();
                eolFailMessages.Add(boardIndex, failMessages);
            }

            List<EolStep.TestResult> eolResults = null;
            if (eolRunResults.ContainsKey(boardIndex))
            {
                eolResults = eolRunResults[boardIndex];
            }

            var headerBuilder = new StringBuilder();
            var createdLog = ElozResultLogFormatter.CreateLog(eloz1Device.CurrentProject, eloz1Device?.TestResult, eolResults, addHeader, headerBuilder, failMessages);
            log.Insert(0, headerBuilder);
            if (addHeader)
            {
                log.AppendLine();
            }
            log.Append(createdLog);

            // EOL에 대해서는 사양서 비교를 위한 별도의 로그를 만든다.
            if (isEOL)
            {
                key = boardIndex + EolSpecLogStartIndex;
                if (!projectLogs.ContainsKey(key))
                {
                    log = new StringBuilder();
                    projectLogs.Add(key, log);
                    addHeader = true;

                    if (eolSpecInfo.ContainsKey(key))
                    {
                        eolSpecInfo[key] = (0, 0);
                    }
                    else
                    {
                        eolSpecInfo.Add(key, (0, 0));
                    }
                }
                else
                {
                    log = projectLogs[key];
                    addHeader = false;
                }

                int recordCount = eolSpecInfo[key].RecordCount;
                long totalTime = eolSpecInfo[key].TotalTime;

                // EOL header info.
                log.Insert(0, headerBuilder);

                var ictSpecLog = ElozResultLogFormatter.CreateEOLSpecLog(addHeader, eloz1Device.TestResult, ref recordCount, ref totalTime);
                log.Append(ictSpecLog);

                var eolSpecLog = ElozResultLogFormatter.CreateEOLSpecLog(false, eolResults, ref recordCount, ref totalTime);
                log.Append(eolSpecLog);

                eolSpecInfo[key] = (recordCount, totalTime);
            }

            // Debugging.
            //projectLog.Append(ElozResultLogFormatter.CreateDetailLog(eloz1Device?.TestResult));
        }

        /// <summary>
        /// 테스트 결과를 저장한다.
        /// </summary>
        /// <param name="passed">PASS/FAIL 여부.</param>
        internal List<string> SaveTestLog(bool passed, bool?[,] sectionPassed, string barCode, Product currentProduct, string printedLog,
            long[,] boardTestTimes, int eolFirstSectionIndex)
        {
            var createdLogFiles = new List<string>();
            var testTime = DateTime.Now;
            var logMode = AppSettings.LogFileMode;
            var fileExtension = logMode != LogFileMode.CSV ? "txt" : "csv";

            string logFolderPath = $"{testTime:yyyy}";
            logFolderPath = Path.Combine(logFolderPath, $"{testTime:MM}");
            logFolderPath = Path.Combine(logFolderPath, $"{testTime:dd}");

            foreach (var log in projectLogs)
            {
                SaveLog(log.Key, log.Value, currentProduct, barCode, sectionPassed, testTime, logFolderPath, fileExtension,
                    createdLogFiles, logMode, boardTestTimes, eolFirstSectionIndex);
            }

            string fileName = $"Log_[{(passed ? "OK" : "NG")}]{barCode}_{testTime:HH}_{testTime:mm}_{testTime:ss}.txt";
            string printedLogFilePath = Path.Combine(AppSettings.PrintLogFolder, fileName);
            SavePrintLog(printedLogFilePath, printedLog);

            createdLogFiles.Insert(0, printedLogFilePath);
            return createdLogFiles;
        }

        internal void SaveLog(int key, StringBuilder log, Product currentProduct, string barCode, bool?[,] sectionPassed, DateTime testTime,
            string logFolderPath, string fileExtension, List<string> createdLogFiles, LogFileMode logMode,
            long[,] boardTestTimes, int eolFirstSectionIndex)
        {
            // Local, Other 로그는 Project/OK or NG 폴더 생성.
            var projectName = Path.GetFileNameWithoutExtension(currentProduct.Project.Path);
            int underBarIndex = projectName.IndexOf('_');

            // Save the log.
            bool isEol = false;
            int boardIndex = key;
            bool boardPassed;
            if (key >= EolSpecLogStartIndex)
            {
                isEol = true;
                boardIndex = key - EolSpecLogStartIndex;
                var totalTime = eolSpecInfo[key].TotalTime;

                string barcodeNumber = IncreaseBarcode(barCode, boardIndex);
                var headerBuilder = new StringBuilder();
                headerBuilder.AppendLine("EOL_GND Tester,,,,,,");
                headerBuilder.AppendLine("SN : " + barcodeNumber + ",,,,,,");
                headerBuilder.AppendLine("BOARD_TYPE : " + currentProduct.Project.Model + ",,,,,,");
                headerBuilder.AppendLine("CAR_TYPE : " + currentProduct.CarType + ",,,,,,");
                headerBuilder.AppendLine("DATE : " + testTime.ToString("yyyy-MM-dd HH:mm:ss") + ",,,,,,");
                headerBuilder.AppendLine("LINE NO : " + AppSettings.MesLineId + ",,,,,,");
                headerBuilder.AppendLine("PROCESS : " + AppSettings.MesPcId + ",,,,,,");
                int lastSectionIndex = eolFirstSectionIndex >= 0 ? eolFirstSectionIndex : sectionPassed.GetLength(0);
                boardPassed = CheckBoardPassed(sectionPassed, boardIndex, lastSectionIndex, false);
                headerBuilder.AppendLine("RESULT : " + (boardPassed ? "OK" : "NG") + ",,,,,,");
                headerBuilder.AppendLine("TEST Zone : ,,,,,,");

                // EOL 테스트 시간 계산.
                long eolTestTime = 0;
                if (eolFirstSectionIndex >= 0)
                {
                    for (int sectionIndex = eolFirstSectionIndex; sectionIndex < boardTestTimes.GetLength(0); sectionIndex++)
                    {
                        eolTestTime += boardTestTimes[sectionIndex, boardIndex];
                    }
                }

                headerBuilder.AppendLine($"Cycle Time : {eolTestTime / 1000.0:0.#}" + " s,,,,,,");
                //headerBuilder.AppendLine();

                log.Insert(0, headerBuilder);
            }
            else
            {
                if (key >= EolIctLogStartIndex)
                {
                    isEol = true;
                    boardIndex = key - EolIctLogStartIndex;
                }

                var headerBuilder = new StringBuilder();
                int lastSectionIndex = eolFirstSectionIndex >= 0 ? eolFirstSectionIndex : sectionPassed.GetLength(0);
                boardPassed = CheckBoardPassed(sectionPassed, boardIndex, lastSectionIndex, !isEol);
                headerBuilder.AppendLine("TOTAL_RESULT: " + (boardPassed ? "PASS" : "FAIL"));
                headerBuilder.AppendLine("DATE: " + testTime.ToString("yyyy-MM-dd HH:mm:ss"));
                if (isEol && eolFailMessages.ContainsKey(boardIndex))
                {
                    var failMessages = eolFailMessages[boardIndex];
                    headerBuilder.AppendLine("SUB_RESULT: " + string.Join(", ", failMessages));
                }

                // ICT, EOL 테스트 시간 계산.
                long ictElapsedMilliseconds = 0;
                long eolElapsedMilliseconds = 0;
                long totalMilliseconds = 0;
                for (int sectionIndex = 0; sectionIndex < boardTestTimes.GetLength(0); sectionIndex++)
                {
                    if (eolFirstSectionIndex < 0 || sectionIndex < eolFirstSectionIndex)
                    {
                        ictElapsedMilliseconds += boardTestTimes[sectionIndex, boardIndex];
                    }
                    else
                    {
                        eolElapsedMilliseconds += boardTestTimes[sectionIndex, boardIndex];
                    }

                    totalMilliseconds += boardTestTimes[sectionIndex, boardIndex];
                }

                headerBuilder.AppendLine("ICT_TEST_TIME: " + ictElapsedMilliseconds + "ms");
                headerBuilder.AppendLine("EOL_TEST_TIME: " + eolElapsedMilliseconds + "ms");
                headerBuilder.AppendLine("TOTAL_TEST_TIME: " + totalMilliseconds + "ms");
                log.Insert(0, headerBuilder);

                log.AppendLine();
                log.AppendLine(projectName);
            }

            string boardBarcode = IncreaseBarcode(barCode, boardIndex);
            string suffix;
            if (boardPassed)
            {
                suffix = "OK";
            }
            else
            {
                suffix = "NG";
            }

            // 파일이름 = FGCode_Barcode_yyyy_MMdd_HHmmss_{OK|NG}.txt

            // 품번에서 경로에 포함될 수 없는 문자 처리.
            var fgCode = currentProduct.FGCode?.Trim() ?? "";
            var fgCodeBuilder = new StringBuilder(fgCode);
            while (true)
            {
                int invalidCharIndex = fgCode.IndexOfAny(Path.GetInvalidFileNameChars());
                if (invalidCharIndex < 0)
                {
                    break;
                }

                fgCodeBuilder[invalidCharIndex] = '_';
            }
            fgCode = fgCodeBuilder.ToString();

            // 파일이름.
            //string logFileName = $"[{prefix}]{boardBarcode}_{testTime:HH}_{testTime:mm}_{testTime:ss}";
            string logFileName = $"{fgCode}_{boardBarcode}_{testTime:yyyy_MMdd_HHmmss}_{suffix}";
            if (key >= EolSpecLogStartIndex)
            {
                logFileName += ".csv";
            }
            else
            {
                string fileNameSuffix = isEol ? "_full.txt" : ("." + fileExtension);
                logFileName += fileNameSuffix;
            }

            // Local, other 로그는 연/월/일/{ICT | EOL}/프로젝트/{OK | NG}/ 폴더에 저장.
            // MES 로그는 연/월/일/{OK | NG}/ 폴더에 저장.
            string ictEolFolder = isEol ? "EOL" : "ICT";
            bool testPassed = isEol ? CheckEolPassed(sectionPassed, eolFirstSectionIndex) : CheckIctPassed(sectionPassed, eolFirstSectionIndex);
            string okNgFolder = testPassed ? "OK" : "NG";

            string localLogFilePath = Path.Combine(AppSettings.TestLogLocalFolder, logFolderPath, ictEolFolder, projectName, okNgFolder, logFileName);
            string otherLogFilePath = Path.Combine(AppSettings.TestLogOtherFolder, logFolderPath, ictEolFolder, projectName, okNgFolder, logFileName);
            List<string> logFiles = new List<string> { localLogFilePath, otherLogFilePath };

            if (AppSettings.MesEnabled)
            {
                ictEolFolder = isEol ? "" : "ICT";
                string mesLogFilePath = Path.Combine(AppSettings.TestLogMesFolder, logFolderPath, ictEolFolder, okNgFolder, logFileName);
                logFiles.Add(mesLogFilePath);
            }

            createdLogFiles.Add(localLogFilePath);

            // EOL 스코프 이미지 리스트.
            var imageRecords = new List<EolStep.TestResult>();
            if (eolRunResults.ContainsKey(boardIndex))
            {
                foreach (var record in eolRunResults[boardIndex])
                {
                    if (record.ResultData is Image)
                    {
                        imageRecords.Add(record);
                    }
                }
            }

            foreach (string logFile in logFiles)
            {
            save_log_file:
                try
                {
                    string folderPath = Path.GetDirectoryName(logFile);
                    Directory.CreateDirectory(folderPath);

                    if (logMode == LogFileMode.ZIP)
                    {
                        // .zip 파일 저장.
                        string zipFileName = Path.GetFileNameWithoutExtension(logFile);
                        string zipFilePath = Path.Combine(folderPath, zipFileName + ".zip");
                        using (var zipStream = new FileStream(zipFilePath, FileMode.Create))
                        {
                            using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                            {
                                var entry = zipArchive.CreateEntry(Path.GetFileName(logFile));
                                using (var writer = new StreamWriter(entry.Open()))
                                {
                                    writer.WriteLine(log);
                                }
                            }
                        }
                    }
                    else
                    {
                        // .txt, .csv 파일 저장.
                        File.WriteAllText(logFile, log.ToString(), Encoding.UTF8);
                    }

                    // EOL 스코프 이미지 캡처 파일 만들기.
                    if (imageRecords.Count > 0)
                    {
                        // 이미지들을 저장할 폴더 생성.
                        string imageFolderName = Path.GetFileNameWithoutExtension(logFile) + "_images";
                        string imageFolderPath = Path.Combine(folderPath, imageFolderName);
                        Directory.CreateDirectory(imageFolderPath);

                        // 이미지 저장.
                        for (int i = 0; i < imageRecords.Count; i++)
                        {
                            string imageFileName = $"{i + 1}_{imageRecords[i].Step.StepId}_{imageRecords[i].Step.Name}.png";
                            string escapedFileName = string.Concat(imageFileName.Split(Path.GetInvalidFileNameChars()));
                            string imageFilePath = Path.Combine(imageFolderPath, escapedFileName);
                            var image = imageRecords[i].ResultData as Image;
                            image.Save(imageFilePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogTimedMessage($"시험 이력 저장 오류: {ex.Message}{Environment.NewLine}파일경로: {logFile}");

                    // 실패한 경우 사용자에게 다시 시도할 것인지 물어본다.
                    var result = InformationBox.Show($"시험 이력 저장 오류: {ex.Message}{Environment.NewLine}다시 시도하시겠습니까?",
                        logFile, buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Question);
                    if (result == InformationBoxResult.OK)
                    {
                        goto save_log_file;
                    }

                    throw;
                }
            }
        }

        internal static void SavePrintLog(string logFilePath, string printedLog)
        {
            // 사용자에게 보여준 로그 저장.
            StreamWriter writer = null;
            try
            {
                writer = File.CreateText(logFilePath);
                writer.WriteLine(printedLog);
            }
            catch (Exception e)
            {
                Logger.LogError($"시험 로그 저장 오류: {e.Message}{Environment.NewLine}파일경로: {logFilePath}");
                throw;
            }
            finally
            {
                writer?.Close();
            }
        }

        /// <summary>
        /// Power 1, 2 를 끈다.
        /// </summary>
        internal void Power12Off(bool power1Off, bool power2Off)
        {
            try
            {
                Monitor.Enter(power12Lock);
                if (power1Off)
                {
                    power1Device.Open();
                    power1Device.SetOutput(false);
                }
                if (power2Off)
                {
                    power2Device.Open();
                    power2Device.SetOutput(false);
                }
            }
            finally
            {
                power1Device.Close();
                power2Device.Close();
                Monitor.Exit(power12Lock);
            }
        }

        internal void PlcCylinderFctUp()
        {
            plcDevice.CylinderFctUp();
        }

        internal void DioRWCylinderFctUp()
        {
            dioOutDevice.CylinderFctUp();
        }

        internal void PlcCylinderMidUp()
        {
            plcDevice.CylinderMidUp();
        }

        /// <summary>
        /// Novaflash 디바이스에 연결한다.
        /// </summary>
        internal void NovaOpen()
        {
            lock (novaLockObj)
            {
                if (AppSettings.NovaflashUseLan)
                {
                    novaDevice.OpenLan(AppSettings.NovaflashLanIp);
                }
                else
                {
                    novaDevice.OpenSerial(AppSettings.NovaflashSerialPortName, AppSettings.NovaflashSerialBaudRate);
                }
            }
        }

        /// <summary>
        /// Novaflash 디바이스를 닫는다.
        /// </summary>
        internal void NovaClose()
        {
            novaDevice.Close();
        }

        /// <summary>
        /// Novaflash 디바이스에 GRP 파일을 전송한다.
        /// </summary>
        /// <param name="filePath">전송하려는 파일 경로 이름.</param>
        internal void NovaSendGrpFile(string filePath, string destFileName)
        {
            lock (novaLockObj)
            {
                novaDevice.SendFile(Novaflash.FileType.Grp, filePath, destFileName);
            }
        }

        /// <summary>
        /// Novaflash 디바이스에 GRP 파일이 있는지 체크한다.
        /// </summary>
        /// <param name="fileName">체크하려는 파일 이름(확장자 포함).</param>
        internal bool NovaGrpExists(string fileName, uint grpCrc)
        {
            lock (novaLockObj)
            {
                return novaDevice.GrpExists(fileName, grpCrc, 1000);
            }
        }

        /// <summary>
        /// Novaflash 디바이스에 연결된 POD 들의 전원을 켜거나 끈다.
        /// </summary>
        /// <param name="on"></param>
        /// <param name="channel1"></param>
        /// <param name="channel2"></param>
        /// <param name="channel3"></param>
        /// <param name="channel4"></param>
        internal void NovaPodPower(bool on, bool channel1, bool channel2, bool channel3, bool channel4)
        {
            lock (novaLockObj)
            {
                var channelInfo = new bool[] { channel1, channel2, channel3, channel4 };
                int timeout = 3000;
                for (int i = 0; i < channelInfo.Length; i++)
                {
                    if (channelInfo[i])
                    {
                        novaDevice.PodPower(i + 1, on, timeout);
                    }
                }
            }
        }

        /// <summary>
        /// Novaflash 디바이스의 채널 상태를 얻는다.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        internal Novaflash.ChannelState NovaGetChState(int channel)
        {
            lock (novaLockObj)
            {
                return novaDevice.GetChannelState(channel);
            }
        }

        internal void Print(string barcode, string model, string projName)
        {
            try
            {
                //var strippedText = StripDuplicateSections(text);

                //if (string.IsNullOrEmpty(strippedText))
                //{
                //    return;
                //}

                var textBuilder = new StringBuilder();
                textBuilder.AppendLine($"Model: {model}");
                textBuilder.AppendLine($"S/N: {barcode}");
                textBuilder.AppendLine($"Date: {DateTime.Now.ToString().Trim()}");
                textBuilder.AppendLine($"Proj: {projName}");
                textBuilder.AppendLine(projUnderline);
                textBuilder.Append(notepadLogBuilder);
                textBuilder.AppendLine(projUnderline);

                printingText = textBuilder.ToString();

                PrintDocument pd = new PrintDocument();
                pd.PrintPage += Pd_PrintPage;
                pd.Print();
            }
            catch (Exception e)
            {
                Logger.LogTimedMessage($"Print Error: {e.Message}");
            }
        }

        // Section이 실패한 경우 다시 테스트할 때 마지막 테스트 내용만 출력한다.
        private string StripDuplicateSections(string logText)
        {
            string strippedText = logText;

            int startIndex = 0;
            while (startIndex < strippedText.Length)
            {
                // Section 이름을 찾는다.
                int sectionHeaderStartIndex = strippedText.IndexOf(sectionHeaderMark, startIndex);
                if (sectionHeaderStartIndex >= 0)
                {
                    int sectionHeaderEndIndex = strippedText.IndexOf(Environment.NewLine, sectionHeaderStartIndex);
                    if (sectionHeaderEndIndex >= 0)
                    {
                        string sectionHeader = strippedText.Substring(sectionHeaderStartIndex, sectionHeaderEndIndex - sectionHeaderStartIndex);

                        // 같은 섹션 Header가 있는지 검사한다.
                        int sameSectionHeaderStartIndex = strippedText.IndexOf(sectionHeader, sectionHeaderEndIndex);
                        if (sameSectionHeaderStartIndex >= 0)
                        {
                            // 같은 섹션 Header가 있으면 현재 섹션 로그를 지운다.
                            strippedText = strippedText.Remove(sectionHeaderStartIndex, sameSectionHeaderStartIndex - sectionHeaderStartIndex);
                        }

                        startIndex = sectionHeaderEndIndex;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return strippedText;
        }

        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            var printingFont = new Font("Consolas", AppSettings.PrintFontSize);
            var textSize = e.Graphics.MeasureString(printingText, printingFont, e.PageBounds.Width);
            e.Graphics.DrawString(printingText, printingFont, Brushes.Black, new Rectangle(
                e.PageBounds.X, e.PageBounds.Y, e.PageBounds.Width, (int)Math.Ceiling(textSize.Height)));
        }

        /// <summary>
        /// MES client에 M1 요청 메시지를 전송하고 그 응답을 받아온다.
        /// </summary>
        /// <param name="barcode">바코드 정보.</param>
        /// <param name="response">응답 메시지.</param>
        internal static void MesIctSendM1Message(string barcode, out MesResponseMessage response)
        {
            MesRequestMessage request = new MesRequestMessage();
            request.ProcessFlag = MesMessage.MessageType.M1;
            request.MessageId = AppSettings.MesMessageId;
            request.PcId = AppSettings.MesPcId;
            request.FactoryId = AppSettings.MesFactoryId;
            request.LineId = AppSettings.MesLineId;
            request.OperId = AppSettings.MesOperId;
            request.EquipmentId = AppSettings.MesEquipmentId;
            request.BarcodeType = AppSettings.MesBarcodeType;
            request.BarcodeNo = barcode;
            request.TransactionTime = DateTime.Now;
            MesServer.SharedIctServer.SendRequest(request, out response);
        }

        /// <summary>
        /// MES client에 M3 요청을 보내고 그 응답을 받는다.
        /// </summary>
        /// <param name="barcode">바코드.</param>
        /// <param name="passed">시험 Pass/Fail 결과.</param>
        /// <param name="response">응답 메시지.</param>
        internal static void MesIctSendM3Message(string barcode, bool passed, out MesResponseMessage response)
        {
            MesRequestMessage request = new MesRequestMessage();
            request.ProcessFlag = MesMessage.MessageType.M3;
            request.MessageId = AppSettings.MesMessageId;
            request.PcId = AppSettings.MesPcId;
            request.FactoryId = AppSettings.MesFactoryId;
            request.LineId = AppSettings.MesLineId;
            request.OperId = AppSettings.MesOperId;
            request.EquipmentId = AppSettings.MesEquipmentId;
            request.BarcodeType = AppSettings.MesBarcodeType;
            request.BarcodeNo = barcode;
            request.Status = passed ? MesMessage.StatusCode.OK : MesMessage.StatusCode.NG;
            request.TransactionTime = DateTime.Now;
            MesServer.SharedIctServer.SendRequest(request, out response);
        }

        /// <summary>
        /// MES client에 M1 요청 메시지를 전송하고 그 응답을 받아온다.
        /// </summary>
        /// <param name="barcode">바코드 정보.</param>
        /// <param name="response">응답 메시지.</param>
        internal static void MesEolSendM1Message(string barcode, out MesResponseMessage response)
        {
            MesRequestMessage request = new MesRequestMessage();
            request.ProcessFlag = MesMessage.MessageType.M1;
            request.MessageId = AppSettings.MesMessageId;
            request.PcId = AppSettings.MesPcId;
            request.FactoryId = AppSettings.MesFactoryId;
            request.LineId = AppSettings.MesLineId;
            request.OperId = AppSettings.MesOperId;
            request.EquipmentId = AppSettings.MesEquipmentId;
            request.BarcodeType = AppSettings.MesBarcodeType;
            request.BarcodeNo = barcode;
            request.TransactionTime = DateTime.Now;
            MesServer.SharedEolServer.SendRequest(request, out response);
        }

        /// <summary>
        /// MES client에 M3 요청을 보내고 그 응답을 받는다.
        /// </summary>
        /// <param name="barcode">바코드.</param>
        /// <param name="passed">시험 Pass/Fail 결과.</param>
        /// <param name="response">응답 메시지.</param>
        internal static void MesEolSendM3Message(string barcode, bool passed, out MesResponseMessage response)
        {
            MesRequestMessage request = new MesRequestMessage();
            request.ProcessFlag = MesMessage.MessageType.M3;
            request.MessageId = AppSettings.MesMessageId;
            request.PcId = AppSettings.MesPcId;
            request.FactoryId = AppSettings.MesFactoryId;
            request.LineId = AppSettings.MesLineId;
            request.OperId = AppSettings.MesOperId;
            request.EquipmentId = AppSettings.MesEquipmentId;
            request.BarcodeType = AppSettings.MesBarcodeType;
            request.BarcodeNo = barcode;
            request.Status = passed ? MesMessage.StatusCode.OK : MesMessage.StatusCode.NG;
            request.TransactionTime = DateTime.Now;
            MesServer.SharedEolServer.SendRequest(request, out response);
        }

        /// <summary>
        /// 직렬연결 뒷설비(서버)에 연결되었는지 여부.
        /// </summary>
        /// <returns></returns>
        internal static bool SeriesConnected()
        {
            return SeriesClient.SharedClient.Connected;
        }

        /// <summary>
        /// 직렬연결 뒷설비(서버)에 연결한다.
        /// </summary>
        internal static void SeriesConnect()
        {
            SeriesClient.SharedClient.Connect(AppSettings.SeriesServer, AppSettings.SeriesServerPort);
        }

        /// <summary>
        /// 직렬연결 뒷설비(서버) 연결을 해제한다.
        /// </summary>
        internal static void SeriesDisconnect()
        {
            SeriesClient.SharedClient.Disconnect();
        }

        /// <summary>
        /// 직렬연결 뒷설비(서버)에 여유 처리능력을 문의한다.
        /// </summary>
        internal static int SeriesGetCapacity()
        {
            var request = new SeriesRequestMessage(SeriesMessage.RequestCommand.GetCapacity);
            var response = SeriesClient.SharedClient.SendRequest(request);
            return response.Capacity;
        }

        internal static bool SeriesServerRunning()
        {
            return SeriesServer.SharedServer.Running;
        }

        internal static void SeriesServerStart()
        {
            SeriesServer.SharedServer.Start(AppSettings.SeriesServerPort);
        }

        internal static void SeriesServerStop()
        {
            SeriesServer.SharedServer.Stop();
        }

        /// <summary>
        /// 주어진 Fixture ID에 대한 Probe Count 정보를 가져온다.
        /// </summary>
        /// <param name="fixtureId"></param>
        /// <returns></returns>
        internal static FixtureProbeCount GetProbeCount(int fixtureId)
        {
            if (probeCountManager == null)
            {
                return null;
            }

            FixtureProbeCount count = null;

            foreach (var probeCount in probeCountManager.Probes)
            {
                if (probeCount.FixtureId == fixtureId)
                {
                    count = probeCount;
                    break;
                }
            }

            if (count == null)
            {
                count = new FixtureProbeCount();
                count.FixtureId = fixtureId;
            }

            // Sync, Async 설정에 따라 TotalProbeCount와 MaxProbeCount를 읽어온다.
            if (AppSettings.ProbeCountSyncMode)
            {
                ProbeCounter.ReadCount(out uint currentValue, out uint maxValue);
                count.TotalProbeCount = (int)currentValue;
                count.MaxProbeCount = (int)maxValue;
            }
            else
            {
                count.MaxProbeCount = AppSettings.MaxProbeCount;
            }

            return count;
        }

        /// <summary>
        /// 주어진 Fixture ID에 대한 Probe Count 정보를 저장한다.
        /// </summary>
        /// <param name="fixProbeCount"></param>
        internal static void SaveProbeCount(FixtureProbeCount fixProbeCount)
        {
            foreach (var probeCount in probeCountManager.Probes)
            {
                if (probeCount.FixtureId == fixProbeCount.FixtureId)
                {
                    probeCountManager.Save();
                    return;
                }
            }

            probeCountManager.Probes.Add(fixProbeCount);
            probeCountManager.Save();
        }

        internal void PlayMesSound(bool isM2, bool ok)
        {
            // MES 사운드 플레이 모드.
            var mesSoundMode = AppSettings.MesSoundPlayMode;
            if (mesSoundMode == 0)
            {
                // 오디오 플레이.
                string audioFile;
                if (isM2)
                {
                    audioFile = ok ? AppSettings.MesM2OkSoundFile : AppSettings.MesM2NgSoundFile;
                }
                else
                {
                    audioFile = ok ? AppSettings.MesM4OkSoundFile : AppSettings.MesM4NgSoundFile;
                }
                PlayAudioFile(audioFile);
            }
        }

        /// <summary>
        /// 테스트 시작 시 사운드를 플레이한다.
        /// </summary>
        /// <param name="passed"></param>
        internal void PlayStartSound()
        {
            int playMode = AppSettings.StartSoundMode;
            if (playMode == 0)
            {
                // 사운드 플레이.
                PlayAudioFile(AppSettings.StartSoundFile);
            }
        }

        /// <summary>
        /// 테스트 결과에 따라 사운드를 플레이한다.
        /// </summary>
        /// <param name="passed"></param>
        internal void PlayResultSound(bool passed)
        {
            int playMode = AppSettings.ResultSoundPlayMode;
            if (playMode == 0)
            {
                // 사운드 플레이.
                PlayAudioFile(passed ? AppSettings.ResultPassSoundFile : AppSettings.ResultFailSoundFile);
            }
        }

        internal static void PlayAudioFile(string filePath)
        {
            try
            {
                //if (wmPlayer == null)
                //{
                //    wmPlayer = new WindowsMediaPlayer();
                //}
                //wmPlayer.MediaError += Player_MediaError;
                //wmPlayer.URL = filePath;
                //wmPlayer.controls.play();

                var player = new SoundPlayer();
                player.SoundLocation = filePath;
                player.Play();
            }
            catch (Exception e)
            {
                Logger.LogError($"Play Sound: {e.Message}");
            }
        }

        //private void Player_MediaError(object pMediaObject)
        //{
        //    Logger.LogError($"Sound MediaError");
        //}

        internal bool JtagCheckConnection()
        {
            try
            {
                Cascon.Init();
            }
            catch (Exception e)
            {
                Logger.LogError($"JTAG: {e.Message}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Novaflash 실행을 위한 정보들을 저장한다.
        /// </summary>
        /// <param name="project"></param>
        internal static void SaveNovaflashData(TestProject project)
        {
            var novaData = new NovaflashData
            {
                LanConnection = AppSettings.NovaflashUseLan,
                LanAddress = AppSettings.NovaflashLanIp,
                SerialPortName = AppSettings.NovaflashSerialPortName,
                SerialBaudRate = AppSettings.NovaflashSerialBaudRate,
                GrpFiles = project.GrpFiles,
            };
            novaData.Save(NovaTempFilePath);
            Logger.LogTimedMessage($"Novaflash GRP Temp File: {NovaTempFilePath}");
        }

        /// <summary>
        /// JTAG 프로젝트 이름을 저장한다.
        /// </summary>
        /// <param name="projectName"></param>
        internal static void SaveJtagProjectInfo(string projectName)
        {
            // UUT 이름 저장.
            using (FileStream fs = new FileStream(JtagTempFilePath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(projectName);

                    Logger.LogTimedMessage($"JTAG Temp File: {JtagTempFilePath}");
                }
            }
        }

        /// <summary>
        /// JTAG 프로젝트 이름, 바코드를 저장한다.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="barcode"></param>
        internal static void SaveJtagBarcodeInfo(int board, string barcode)
        {

            // 바코드 저장.
            string tempFolderPath = Path.GetTempPath();
            string tempFileName = "SERIALNR.DAT";
            string tempFilePath = Path.Combine(tempFolderPath, tempFileName);
            using (FileStream fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine($"C {board}");
                    sw.WriteLine($"S {barcode}");

                    Logger.LogTimedMessage($"JTAG SerialNr File: {tempFilePath}");
                }
            }
        }

        /// <summary>
        /// Barcode를 파싱해 제일 마지막 수를 증가시킨 바코드를 만들어 리턴한다.
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        internal static string IncreaseBarcode(string barcode, int boardIndex)
        {
            if (boardIndex == 0 || string.IsNullOrEmpty(barcode))
            {
                return barcode;
            }

            // 바코드 순서와 보드 순서가 다르다.
            int barcodeIndex = boardIndex;
            if (boardIndex == 2)
            {
                barcodeIndex = 3;
            }
            else if (boardIndex == 3)
            {
                barcodeIndex = 2;
            }

            // 마지막에서 4번째부터 3자리가 16진수 시리얼 번호이다.
            int serialNrStartIndex = barcode.Length - 4;
            int serialNrLength = 3;
            string serialNrText = barcode.Substring(serialNrStartIndex, serialNrLength);
            int serialNr = int.Parse(serialNrText, System.Globalization.NumberStyles.HexNumber);
            serialNr += barcodeIndex;
            string newSerialNrText = string.Format($"{{0:X{serialNrLength}}}", serialNr);
            string newBarcode = barcode.Substring(0, serialNrStartIndex) + newSerialNrText + barcode.Substring(serialNrStartIndex + serialNrLength);
            return newBarcode;
        }

        internal static void AddTestHistory(TestHistory history, FixtureProbeCount probeCount)
        {
            if (AppSettings.HistoryKeepingPeriod <= 0)
            {
                return;
            }
            TestHistoryContext context = null;
            try
            {
                context = new TestHistoryContext();

                // 그룹 체크.
                var groups = context.HistoryGroups.ToList();
                HistoryGroup group;
                if (groups.Count == 0)
                {
                    group = new HistoryGroup()
                    {
                        StartTime = history.StartTime,
                    };
                    context.HistoryGroups.Add(group);
                    context.SaveChanges();
                }
                else
                {
                    group = groups.OrderByDescending(g => g.StartTime).First();
                }

                if (probeCount != null)
                {
                    group.TestCount = probeCount.TodayTestCount;
                    group.PassCount = probeCount.TodayPassCount;
                }

                // History 추가.
                history.HistoryGroupId = group.Id;
                context.TestHistories.Add(history);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                string message = "Test Hisotry: " + ex.Message;
                if (ex.InnerException != null)
                {
                    message += ": " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        message += ": " + ex.InnerException.InnerException.Message;
                    }
                }
                Logger.LogError(message);
            }
            finally
            {
                context?.Dispose();
            }
        }

        internal static void RemoveOldHistory()
        {
            TestHistoryContext dbContext = null;
            try
            {
                dbContext = new TestHistoryContext();
                int period = AppSettings.HistoryKeepingPeriod;
                var periodUnit = AppSettings.HistoryKeepingUnit;
                if (period <= 0)
                {
                    // 이력 모두 지우기.
                    dbContext.Database.ExecuteSqlCommand($"DELETE FROM {nameof(TestHistoryContext.TestHistories)}");
                    dbContext.Database.ExecuteSqlCommand($"DELETE FROM {nameof(TestHistoryContext.HistoryGroups)}");
                }
                else
                {
                    // 이력 지우기.
                    int totalDays;
                    switch (periodUnit)
                    {
                        case TimespanUnit.Days:
                            totalDays = period + 1;
                            break;
                        case TimespanUnit.Weeks:
                            totalDays = period * 7 + 1;
                            break;
                        case TimespanUnit.Years:
                            totalDays = period * 365 + 1;
                            break;
                        case TimespanUnit.Months:
                        default:
                            totalDays = period * 31 + 1;
                            break;
                    }
                    var oldDate = DateTime.Now.AddDays(-totalDays);
                    var oldHistories = dbContext.TestHistories.Where(h => h.StartTime < oldDate);
                    if (oldHistories.Count() > 0)
                    {
                        var oldGroups = oldHistories.Select(h => h.HistoryGroupId).ToHashSet();
                        dbContext.TestHistories.RemoveRange(dbContext.TestHistories.Where(h => oldGroups.Contains(h.HistoryGroupId)));

                        var deletingGroups = dbContext.HistoryGroups.Where(g => oldGroups.Contains(g.Id));
                        dbContext.HistoryGroups.RemoveRange(deletingGroups);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Hisotry Remove: " + ex.Message;
                if (ex.InnerException != null)
                {
                    message += ": " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        message += ": " + ex.InnerException.InnerException.Message;
                    }
                }
                Logger.LogError(message);
            }
            finally
            {
                dbContext?.Dispose();
            }
        }

        internal static void AddHistoryGroup()
        {
            TestHistoryContext context = null;
            try
            {
                context = new TestHistoryContext();
                var newGroup = new HistoryGroup() { StartTime = DateTime.Now };
                context.HistoryGroups.Add(newGroup);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                string message = "Hisotry Group: " + ex.Message;
                if (ex.InnerException != null)
                {
                    message += ": " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        message += ": " + ex.InnerException.InnerException.Message;
                    }
                }
                Logger.LogError(message);
            }
            finally
            {
                context?.Dispose();
            }
        }

        internal static void ResetHistory()
        {
            TestHistoryContext context = null;
            try
            {
                context = new TestHistoryContext();
                context.Database.ExecuteSqlCommand($"TRUNCATE TABLE {nameof(context.TestHistories)}");
                context.Database.ExecuteSqlCommand($"TRUNCATE TABLE {nameof(context.HistoryGroups)}");
            }
            catch (Exception ex)
            {
                string message = "Reset Hisotry: " + ex.Message;
                if (ex.InnerException != null)
                {
                    message += ": " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        message += ": " + ex.InnerException.InnerException.Message;
                    }
                }
                Logger.LogError(message);
            }
            finally
            {
                context?.Dispose();
            }
        }

        internal void ShowPcbViewer(TestProject project)
        {
            try
            {
                if (!File.Exists(AppSettings.PcbViewerPath))
                {
                    return;
                }

                // Arguments.
                var arguments = new StringBuilder();

                // PCB info file.
                arguments.Append($"\"{project?.PcbInfoFile ?? ""}\"");

                var testData = eloz1Device.CurrentProject.GetItem("TestData");

                // Failed list.
                var runResult = eloz1Device.CurrentProject.GetTestResult().TestRunResult;
                for (int resultIndex = 0; resultIndex < runResult.Count; resultIndex++)
                {
                    if (runResult[resultIndex] is TestStepResultRecord stepRecord)
                    {
                        for (int itemIndex = 0; itemIndex < stepRecord.Count; itemIndex++)
                        {
                            var itemResult = stepRecord[itemIndex];
                            var testStep = testData.GetItem("TestStep", itemResult.TestStepRecordID);
                            string templateName = testStep.GetProperty("TemplateName").ToString();
                            if (itemResult.ResultState == ResultState.Fail && !templateName.Equals("Script", StringComparison.OrdinalIgnoreCase))
                            {
                                string part;
                                StringComparison templateComparison = StringComparison.OrdinalIgnoreCase;
                                if (templateName.Equals("Script", templateComparison) ||
                                    templateName.Equals("DischargeTest", templateComparison) ||
                                    templateName.Equals("ContactTest", templateComparison) ||
                                    templateName.Equals("ShortsTest", templateComparison) ||
                                    templateName.Equals("OpensTest", templateComparison))
                                {
                                    part = "";
                                }
                                else
                                {
                                    var delimiters = new string[] { "+", "-", "_", "/", " " };
                                    var splitted = itemResult.TestStepName.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                                    if (splitted.Length > 0)
                                    {
                                        part = splitted[0].Trim();
                                    }
                                    else
                                    {
                                        part = "";
                                    }
                                }
                                arguments.Append($" \"Board={itemResult.BoardMultiPanelID}|Part={part}|Pin={itemResult.PartPinDesignator}" +
                                    $"|Nail={itemResult.FixtureNailNumber}|NodeA={itemResult.NodeAName}|NodeB={itemResult.NodeBName}\"");
                            }
                        }
                    }
                }

                // Execute the PCB viewer.
                Utils.StartProcess(AppSettings.PcbViewerPath, System.Diagnostics.ProcessWindowStyle.Normal, arguments.ToString());
            }
            catch (Exception ex)
            {
                Logger.LogError("PCB Viewer: " + ex.Message);
            }
        }

        internal void ClearNotepadLog()
        {
            notepadLogBuilder.Clear();
        }

        internal void CreateNotepadLog()
        {
            // ICT Fail Log.
            for (int recordIndex = 0; recordIndex < eloz1Device.TestResult.TestRunResult.Count; recordIndex++)
            {
                var record = eloz1Device.TestResult.TestRunResult[recordIndex];
                switch (record)
                {
                    case BeginSectionTestResultRecord beginSectionRecord:
                        // Section 헤더.
                        notepadLogBuilder.AppendLine("Section: " + beginSectionRecord.SectionName);
                        notepadLogBuilder.AppendLine(sectionUnderline);
                        break;
                    case TestStepResultRecord stepRecord:
                        for (int i = 0; i < stepRecord.Count; i++)
                        {
                            var itemResult = stepRecord[i];
                            if (AppSettings.PrintingMode == PrintingOptions.FailOnly && itemResult.ResultState != ResultState.Fail 
                                && itemResult.ResultState != ResultState.Aborted)
                            {
                                continue;
                            }

                            // Step 로그 정보.
                            notepadLogBuilder.Append(CreateIctLogRecord(itemResult));
                        }
                        break;
                    case EndSectionTestResultRecord _:
                        // Section 푸터.
                        notepadLogBuilder.AppendLine(sectionUnderline);
                        break;
                }
            }
        }

        internal void CreateEolNotepadLog(string sectionName)
        {
            // EOL Fail Log.
            List<EolStep.TestResult> results = null;
            if (eolRunResults.ContainsKey(EolBoardIndex))
            {
                results = eolRunResults[EolBoardIndex];
            }

            if (results == null)
            {
                return;
            }

            bool logged = false;
            var textBuilder = new StringBuilder();
            foreach (var result in results)
            {
                if (AppSettings.PrintingMode == PrintingOptions.FailOnly && result.ResultState != EolStep.ResultState.Fail 
                    && result.ResultState != EolStep.ResultState.Aborted)
                {
                    continue;
                }

                // Step 로그 정보.
                textBuilder.Append(CreateEolLogRecord(result));
                logged = true;
            }

            if (logged)
            {
                // Section 이름.
                textBuilder.Insert(0, sectionUnderline + Environment.NewLine);
                textBuilder.Insert(0, "Section: " + sectionName + Environment.NewLine);
                textBuilder.AppendLine(sectionUnderline);
            }

            notepadLogBuilder.Append(textBuilder);
        }

        internal void ShowFailInfoNotepad(Form activeForm, string barcode, string model, string projName)
        {
            try
            {
                var logTextBuilder = new StringBuilder();
                logTextBuilder.AppendLine($"Model: {model}");
                logTextBuilder.AppendLine($"S/N: {barcode}");
                logTextBuilder.AppendLine($"Date: {DateTime.Now.ToString().Trim()}");
                logTextBuilder.AppendLine($"Proj: {projName}");
                logTextBuilder.AppendLine(projUnderline);
                logTextBuilder.Append(notepadLogBuilder);
                logTextBuilder.AppendLine(projUnderline);
                Utils.AppendTextToNotepad(logTextBuilder.ToString(), true, activeForm);
            }
            catch (Exception ex)
            {
                Logger.LogError("Show notepad: " + ex.Message);
            }
        }

        /// <summary>
        /// 테스트 결과를 체크해 Open, Short 테스트 Fail 인지 체크.
        /// </summary>
        /// <returns>뒤의 보드를 계속 테스트해야 하면 true, 아니면 false.</returns>
        internal bool CheckToContinueTest()
        {
            for (int recordIndex = 0; recordIndex < eloz1Device.TestResult.TestRunResult.Count; recordIndex++)
            {
                var record = eloz1Device.TestResult.TestRunResult[recordIndex];
                switch (record)
                {
                    case TestStepResultRecord stepRecord:
                        for (int i = 0; i < stepRecord.Count; i++)
                        {
                            var itemResult = stepRecord[i];
                            if (itemResult.ResultState != ResultState.Fail)
                            {
                                continue;
                            }

                            var testData = eloz1Device.CurrentProject.GetItem("TestData");
                            var testStep = testData.GetItem("TestStep", itemResult.TestStepRecordID);

                            // Open, Short 테스트인가 검사.
                            string templateName = testStep.GetProperty("TemplateName").ToString();
                            StringComparison templateComparison = StringComparison.OrdinalIgnoreCase;
                            if (templateName.Equals("ShortsTest", templateComparison) ||
                                templateName.Equals("OpensTest", templateComparison))
                            {
                                return false;
                            }
                        }
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Novaflash 임시파일에 보드들의 Pass/Fail 여부를 저장한다.
        /// </summary>
        /// <param name="boardPassed"></param>
        internal static void SaveBoardStatusForNova(IEnumerable<bool?> boardPassed)
        {
            string line = "";
            foreach (bool? passed in boardPassed)
            {
                string passedText;
                if (passed == true)
                {
                    passedText = "1";
                }
                else
                {
                    passedText = "0";
                }

                if (line == "")
                {
                    line = passedText;
                }
                else
                {
                    line += " " + passedText;
                }
            }
            line += Environment.NewLine;

            File.AppendAllText(NovaTempFilePath, line);

            Logger.LogTimedMessage("Board status saved for novaflash");
        }

        /// <summary>
        /// 해당 차종코드가 포함되었는지 체크.
        /// </summary>
        /// <param name="productCarTypeCodes"></param>
        /// <param name="carTypeCode"></param>
        /// <returns></returns>
        internal static bool ContainsCarTypeCode(string productCarTypeCodes, string carTypeCode)
        {
            if (productCarTypeCodes == null || carTypeCode == null)
            {
                return false;
            }

            bool contains = false;
            string[] codes = productCarTypeCodes.Split(',');
            foreach (string code in codes)
            {
                if (code.Trim().Equals(carTypeCode, StringComparison.OrdinalIgnoreCase))
                {
                    contains = true;
                    break;
                }
            }

            return contains;
        }

        /// <summary>
        /// 바코드의 시리얼 넘버를 리턴한다.
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        internal static int GetBarcodeSerialNumber(string barcode)
        {
            int serialNrStartIndex = barcode.Length - 4;
            int serialNrLength = 3;
            string serialNrText = barcode.Substring(serialNrStartIndex, serialNrLength);
            return int.Parse(serialNrText, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// 테스트 결과를 체크해 Open, Short 테스트 Fail 인지 체크.
        /// </summary>
        /// <returns>Open 또는 Short 테스트 불량이면 true, 아니면 false.</returns>
        internal bool IsOpenShortFail()
        {
            for (int recordIndex = 0; recordIndex < eloz1Device.TestResult.TestRunResult.Count; recordIndex++)
            {
                var record = eloz1Device.TestResult.TestRunResult[recordIndex];
                switch (record)
                {
                    case TestStepResultRecord stepRecord:
                        for (int i = 0; i < stepRecord.Count; i++)
                        {
                            var itemResult = stepRecord[i];
                            if (itemResult.ResultState != ResultState.Fail)
                            {
                                continue;
                            }

                            var testData = eloz1Device.CurrentProject.GetItem("TestData");
                            var testStep = testData.GetItem("TestStep", itemResult.TestStepRecordID);

                            // Open, Short 테스트인가 검사.
                            string templateName = testStep.GetProperty("TemplateName").ToString();
                            StringComparison templateComparison = StringComparison.OrdinalIgnoreCase;
                            if (templateName.Equals("ShortsTest", templateComparison) ||
                                templateName.Equals("OpensTest", templateComparison))
                            {
                                return true;
                            }
                        }
                        break;
                }
            }

            return false;
        }

        /// <summary>
        /// 누적된 테스트 결과로부터 전체 테스트의 pass 여부를 판단한다.
        /// </summary>
        /// <param name="sectionPassed"></param>
        /// <returns></returns>
        internal static bool CheckTestPassed(bool?[,] sectionPassed)
        {
            bool passed = true;
            foreach (var boardPassed in sectionPassed)
            {
                if (boardPassed == false)
                {
                    passed = false;
                    break;
                }
            }
            return passed;
        }

        /// <summary>
        /// 누적된 테스트 결과로부터 ICT 테스트의 pass 여부를 판단한다.
        /// </summary>
        /// <param name="sectionPassed"></param>
        /// <returns></returns>
        internal static bool CheckIctPassed(bool?[,] sectionPassed, int eolFirstSectionIndex)
        {
            bool passed = true;
            for (int sectionIndex = 0; sectionIndex < sectionPassed.GetLength(0); sectionIndex++)
            {
                if (eolFirstSectionIndex >= 0 && sectionIndex >= eolFirstSectionIndex)
                {
                    break;
                }

                for (int boardIndex = 0; boardIndex < sectionPassed.GetLength(1); boardIndex++)
                {
                    if (sectionPassed[sectionIndex, boardIndex] == false)
                    {
                        passed = false;
                        break;
                    }
                }
            }
            return passed;
        }

        /// <summary>
        /// 누적된 테스트 결과로부터 EOL 테스트의 pass 여부를 판단한다.
        /// </summary>
        /// <param name="sectionPassed"></param>
        /// <returns></returns>
        internal static bool CheckEolPassed(bool?[,] sectionPassed, int eolFirstSectionIndex)
        {
            // EOL 테스트를 하지 않으면 true 리턴.
            if (eolFirstSectionIndex < 0)
            {
                return true;
            }

            bool passed = true;
            for (int sectionIndex = eolFirstSectionIndex; sectionIndex < sectionPassed.GetLength(0); sectionIndex++)
            {
                for (int boardIndex = 0; boardIndex < sectionPassed.GetLength(1); boardIndex++)
                {
                    if (sectionPassed[sectionIndex, boardIndex] == false)
                    {
                        passed = false;
                        break;
                    }
                }
            }
            return passed;
        }

        /// <summary>
        /// 누적된 테스트 결과로부터 지정한 번호의 phase가 pass인가를 판단한다.
        /// </summary>
        /// <param name="sectionPassed"></param>
        /// <param name="phaseIndex"></param>
        /// <returns></returns>
        internal static bool CheckPhasePassed(bool?[,] sectionPassed, int phaseIndex)
        {
            bool passed = true;
            for (int boardIndex = 0; boardIndex < sectionPassed.GetLength(1); boardIndex++)
            {
                if (sectionPassed[phaseIndex, boardIndex] == false)
                {
                    passed = false;
                    break;
                }
            }
            return passed;
        }

        /// <summary>
        /// 누적된 테스트 결과로부터 지정한 번호를 가진 보드가 Pass 인가를 판단.
        /// </summary>
        /// <param name="sectionPassed"></param>
        /// <param name="boardIndex"></param>
        /// <returns></returns>
        internal static bool CheckBoardPassed(bool?[,] sectionPassed, int boardIndex, int eolFirstSectionIndex, bool isICT)
        {
            bool passed = true;

            int startIndex = isICT ? 0 : eolFirstSectionIndex;
            int lastIndex = isICT ? eolFirstSectionIndex : sectionPassed.GetLength(0);

            for (int phaseIndex = startIndex; phaseIndex < lastIndex; phaseIndex++)
            {
                if (sectionPassed[phaseIndex, boardIndex] == false)
                {
                    passed = false;
                    break;
                }
            }
            return passed;
        }

        /// <summary>
        /// GRP 파일에 Import 파일을 추가하여 새로운 GRP 생성.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string CreateGRP(string grpFilePath, string importFilePath)
        {
            // Base GRP 파일이 있는가 검사.
            if (!File.Exists(grpFilePath))
            {
                throw new Exception($"GRP file does not exist: {grpFilePath}");
            }

            // Import 파일이 비었으면 GRP 파일을 새로 생성하지 않고 Base GRP를 사용하도록 한다.
            if (string.IsNullOrWhiteSpace(importFilePath))
            {
                return null;
            }

            // Import 파일이 있는가 검사.
            if (!File.Exists(importFilePath))
            {
                throw new Exception($"Import file does not exist: {importFilePath}");
            }

            // 지정한 파일을 추가하여 새로운 GRP 생성.
            var importFileName = Path.GetFileName(importFilePath);
            var outputGrpName = Path.ChangeExtension(importFileName, ".out.grp");
            var outputFoler = "D:\\ElozPlugin\\Temp";
            Directory.CreateDirectory(outputFoler);
            var outputGrpPath = Path.Combine(outputFoler, outputGrpName);
            Novaflash.CreateGRP(grpFilePath, outputGrpPath, importFilePath, "");
            return outputGrpPath;
        }

        /// <summary>
        /// Hydra 디바이스에 업로드된 파일들을 제거한다.
        /// </summary>
        /// <param name="grpList"></param>
        public void DeleteHydraFiles(List<GrpInfo> grpList)
        {
            NovaOpen();

            foreach (var grpInfo in grpList)
            {
                if (!string.IsNullOrWhiteSpace(grpInfo.GrpFilePath))
                {
                    var uploadedFileName = GrpInfo.GetUploadFileName(grpInfo.GrpFilePath, grpInfo.ImportFilePath);
                    bool deleted = novaDevice.DeleteFile(Novaflash.FileType.Grp, uploadedFileName, 500);
                    Logger.LogInfo($"Deleting {uploadedFileName} from Hydra device : {(deleted ? "OK" : "NG")}");
                }
            }
        }
    }
}
