using BrightIdeasSoftware;
using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model;
using EOL_GND.Properties;
using EOL_GND.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestFramework.PluginTestCell.TestResults;

namespace EOL_GND.ViewModel
{
    public class SequenceViewModel
    {
        /// <summary>
        /// EOL ROM ID 체크를 위한 글로벌 변수 이름.
        /// </summary>
        public const string EOLRomIdVarName = "EOLRomId";

        /// <summary>
        /// 현재 테스트하는 보드가 마스터 보드인지 여부를 저장하는 글로벌 변수 이름.
        /// </summary>
        public const string IsMasterVarName = "IsMasterBoard";

        /// <summary>
        /// EOL Spec 용 로그 저장을 위한 패턴 문자열.
        /// eloZ1 TestBuilder 스텝 이름이 이 문자열로 시작하면, 이 스텝의 결과는 EOL Spec 로그에 저장된다.
        /// </summary>
        public const string EOLSpecLogPattern = "#EOL_SPEC#";

        /// <summary>
        /// Clipboard에 스텝들을 저장할 때 이름.
        /// </summary>
        private const string ClipboardFormat = "EOL_GND.StepList";

        public delegate void TestStepRunFinishedEventHandler(EolStep.TestResult result);

        /// <summary>
        /// 테스트 스텝 실행이 끝날 때마다 호출된다.
        /// </summary>
        public static event TestStepRunFinishedEventHandler TestStepRunFinished;

        /// <summary>
        /// 테스트 전체 실행이 끝나면 호출된다.
        /// </summary>
        public static event EventHandler<List<EolStep.TestResult>> TestFinished;

        /// <summary>
        /// 현재 로그인한 유저.
        /// </summary>
        internal User CurrentUser { get; set; }

        internal enum InsertPosition
        {
            Top,
            Current,
            Bottom,
        }

        /// <summary>
        /// 변경된 내용이 있음을 나타내는 표시.
        /// </summary>
        internal const string ModifiedMark = "*";

        /// <summary>
        /// Copy한 스텝들.
        /// </summary>
        internal static List<EolStep> CopiedSteps { get; } = new List<EolStep>();

        /// <summary>
        /// Copy된 스텝들을 이동할 것인지 여부.
        /// </summary>
        internal bool Move { get; private set; } = false;

        /// <summary>
        /// 테스트를 중단할 때 Cleanup 스텝들을 실행할지 여부.
        /// </summary>
        public bool RunCleanup
        {
            get => CurrentSequence?.RunCleanup ?? false;
            set
            {
                if (CurrentSequence != null && CurrentSequence.RunCleanup != value)
                {
                    CurrentSequence.RunCleanup = value;
                    SequenceModified = true;
                }
            }
        }

        /// <summary>
        /// 테스트를 중단할 때 실행할 Cleanup 스텝 ID.
        /// </summary>
        public int[] CleanupSteps
        {
            get => CurrentSequence?.CleanupSteps;
            set
            {
                if (CurrentSequence != null && CurrentSequence.CleanupSteps != value)
                {
                    CurrentSequence.CleanupSteps = value;
                    SequenceModified = true;
                }
            }
        }

        /// <summary>
        /// 시퀀스에서 이용하는 Variant 리스트.
        /// </summary>
        public List<BoardVariant> Variants
        {
            get => CurrentSequence?.Variants;
            set
            {
                if (CurrentSequence != null && CurrentSequence.Variants != value)
                {
                    CurrentSequence.Variants = value;
                    SequenceModified = true;
                }
            }
        }

        /// <summary>
        /// 현재 시퀀스의 파일 경로.
        /// </summary>
        internal string FilePath => CurrentSequence?.FilePath;

        /// <summary>
        /// 현재 로딩된 시퀀스.
        /// </summary>
        private EolSequence CurrentSequence
        {
            get => _currentSequence;
            set
            {
                if (_currentSequence != value)
                {
                    if (OriginalSteps != null)
                    {
                        OriginalSteps.ListChanged -= Steps_ListChanged;
                    }
                    _currentSequence = value;
                    if (_currentSequence != null)
                    {
                        OriginalSteps = new BindingList<EolStep>(_currentSequence.Steps);
                        UpdateStepNo();
                        OriginalSteps.ListChanged += Steps_ListChanged;
                    }
                    else
                    {
                        OriginalSteps = null;
                    }
                    SequenceModified = false;
                }
            }
        }
        private EolSequence _currentSequence = null;

        internal BindingList<EolStep> OriginalSteps { get; private set; }

        /// <summary>
        /// EOL Sequence가 변경되었는지 여부.
        /// </summary>
        internal bool SequenceModified { get; private set; } = false;

        /// <summary>
        /// EOL Sequence가 로딩되었는지 여부.
        /// </summary>
        internal bool SequenceOpened => CurrentSequence != null;

        /// <summary>
        /// EOL Spec 로그 저장(CSV) 시 사용되는 field separator.
        /// </summary>
        private const string specLogFieldSeparator = ",";

        //
        // Application Settings
        //

        /// <summary>
        /// Recently used files.
        /// </summary>
        internal StringCollection MruFiles
        {
            get
            {
                if (Settings.Default.MruFiles == null)
                {
                    Settings.Default.MruFiles = new StringCollection();
                }
                return Settings.Default.MruFiles;
            }
        }

        /// <summary>
        /// 현재 테스트하는 보드가 마스터 보드인지 여부를 글로벌 변수에 저장.
        /// </summary>
        /// <param name="isMasterBoard"></param>
        public static void SetIsMasterBoard(bool? isMasterBoard)
        {
            SetGlobalStorageValue(IsMasterVarName, isMasterBoard);
        }

        /// <summary>
        /// 현재 테스트하는 보드가 마스터 보드인지 여부를 글로벌 변수에서 가져온다.
        /// </summary>
        /// <returns></returns>
        public static bool? GetIsMasterBoard()
        {
            return GetGlobalStorageValue(IsMasterVarName) as bool?;
        }

        /// <summary>
        /// EOL 실행 시 비교할 ROM ID를 설정.
        /// </summary>
        /// <param name="enabled">null이면 해당 글로벌 변수를 지운다.</param>
        public static void SetRomId(string romId)
        {
            SetGlobalStorageValue(EOLRomIdVarName, romId);
        }

        /// <summary>
        /// EOL 실행 시 비교할 ROM ID를 리턴.
        /// </summary>
        /// <returns></returns>
        public static string GetRomId()
        {
            return GetGlobalStorageValue(EOLRomIdVarName) as string;
        }

        private static void SetGlobalStorageValue(string key, object value)
        {
            if (TestFramework.Common.GlobalStorage.Exists(key))
            {
                if (value == null)
                {
                    TestFramework.Common.GlobalStorage.Remove(key);
                    return;
                }
            }
            else
            {
                TestFramework.Common.GlobalStorage.Create(key);
            }

            TestFramework.Common.GlobalStorage.Set(key, value);
        }

        private static object GetGlobalStorageValue(string key)
        {
            object value = null;
            if (TestFramework.Common.GlobalStorage.Exists(key))
            {
                value = TestFramework.Common.GlobalStorage.Get(key);
            }
            return value;
        }

        /// <summary>
        /// Enabled 상태 표시에 사용되는 이미지를 얻는데 사용된다.
        /// </summary>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        internal static string EnabledImageGetter(object rowObject)
        {
            if (rowObject is EolStep step)
            {
                return step.Enabled ? "ok-16.png" : "cancel-16.png";
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 스텝 유형 이미지를 얻는데 사용된다.
        /// </summary>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        internal static string StepImageGetter(object rowObject)
        {
            if (rowObject is EolStep step)
            {
                if (step is EolPowerStep)
                {
                    return "power-16.png";
                }
                else if (step is EolWaveformGeneratorStep)
                {
                    return "waveform-16.png";
                }
                else if (step is EolDmmStep)
                {
                    return "dmm-16.png";
                }
                else if (step is EolOscopeStep)
                {
                    return "oscilloscope-16.png";
                }
                else if (step is EolDioStep)
                {
                    return "microchip-16.png";
                }
                else if (step is EolElozRelayStep)
                {
                    return "relay-16.png";
                }
                else if (step is EolElozStimulusStep)
                {
                    return "stimulus-16.png";
                }
                else if (step is EolElozVoltmeterStep)
                {
                    return "voltmeter-16.png";
                }
                else if (step is EolCanStep)
                {
                    return "can-16.png";
                }
                else if (step is EolLinStep)
                {
                    return "lin-16.png";
                }
                else if (step is EolAbortStep)
                {
                    return "abort-16.png";
                }
                else if (step is EolDummyStep)
                {
                    return "null-16.png";
                }
                else if (step is EolAmplifierStep)
                {
                    return "amplifier-16.png";
                }
                else if (step is EolGloquadEvseStep)
                {
                    return "secc-16.png";
                }
                else if (step is EolSerialPortStep)
                {
                    return "serial_port-16.png";
                }
                else if (step is EolAlphaMotionStep)
                {
                    return "servo-16.png";
                }
                else if (step is EolMightyZapStep)
                {
                    return "mighty_zap-16.png";
                }
            }
            return null;
        }

        /// <summary>
        /// 스텝이 Relay 그룹에 속했는지 여부.
        /// </summary>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        internal static string GroupImageGetter(object rowObject)
        {
            if (rowObject is EolStep step && step.ElozRelayAfter == EolStep.ElozRelayMode.ON)
            {
                return "link-16.png";
            }
            return null;
        }

        /// <summary>
        /// 스텝의 테스트 결과를 Spec Log에 추가할지 여부.
        /// </summary>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        internal static string SpecLogImageGetter(object rowObject)
        {
            if (rowObject is EolStep step && step.ResultSpecLog)
            {
                return "csv-16.png";
            }
            return null;
        }

        internal SequenceViewModel()
        {
        }

        internal void CreateSequence(string filePath)
        {
            CurrentSequence = new EolSequence(filePath);
        }

        /// <summary>
        /// EOL Sequence 파일을 여는데 사용되는 필터.
        /// </summary>
        /// <returns></returns>
        internal static string GetFileFilter()
        {
            return $"{EolSequence.FileDescription} (*.{EolSequence.FileExtension})|*.{EolSequence.FileExtension};" +
                            "|All Files (*.*)|*.*;";
        }

        private void Steps_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged && (e.PropertyDescriptor?.Name == nameof(EolStep.RunResult) || 
                e.PropertyDescriptor?.Name == nameof(EolStep.No)))
            {
            }
            else
            {
                SequenceModified = true;
            }
        }

        /// <summary>
        /// 현재의 시퀀스 저장.
        /// </summary>
        internal void SaveSequence()
        {
            if (CurrentSequence != null)
            {
                CurrentSequence.Save();
                SequenceModified = false;
                UpdateMruList(CurrentSequence.FilePath);
            }
        }

        /// <summary>
        /// 현 시퀀스 다른 이름으로 저장.
        /// </summary>
        /// <param name="filePath"></param>
        internal void SaveAsSequence(string filePath, bool overwrite)
        {
            if (CurrentSequence != null)
            {
                CurrentSequence.SaveAs(filePath);
                SequenceModified = false;
                UpdateMruList(filePath);
            }
        }

        /// <summary>
        /// 지정한 시퀀스 파일 로딩.
        /// </summary>
        /// <param name="filePath"></param>
        internal void LoadSequence(string filePath)
        {
            var loadedSequence = EolSequence.Load(filePath);

            // Id 가 비어있으면(0이면) 자동 할당한다.
            bool idUpdated = false;
            int maxId = 0;
            if (loadedSequence.Steps.Count > 0)
            {
                maxId = loadedSequence.Steps.Select(step => step.Id).Max();
            }

            foreach (var step in loadedSequence.Steps)
            {
                if (step.Id == 0)
                {
                    step.Id = maxId + 1;
                    maxId += 1;

                    idUpdated = true;
                }
            }

            if (idUpdated)
            {
                loadedSequence.Save();
            }

            CurrentSequence = loadedSequence;

            // MRU list 업데이트.
            UpdateMruList(filePath);
        }

        /// <summary>
        /// 시퀀스 닫기.
        /// </summary>
        internal void CloseSequence()
        {
            CurrentSequence = null;
        }

        // 지정한 파일이 없으면 추가하고, 있으면 제일 위로 올린다.
        private void UpdateMruList(string filePath)
        {
            if (!MruFiles.Contains(filePath))
            {
                MruFiles.Insert(0, filePath);
                const int maxMruCount = 10;
                if (MruFiles.Count > maxMruCount)
                {
                    MruFiles.RemoveAt(maxMruCount);
                }
            }
            else
            {
                int index = MruFiles.IndexOf(filePath);
                if (index > 0)
                {
                    MruFiles.RemoveAt(index);
                    MruFiles.Insert(0, filePath);
                }
            }
        }

        /// <summary>
        /// {제품이름} {Major}.{Minor}.{Patch} 형식의 타이틀을 만든다.
        /// </summary>
        /// <returns></returns>
        internal string GetTitle()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var productName = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
            ////var infoVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            //var version = assembly.GetName().Version;

            // elowerk TestBuilder 스크립트가 버전 변경 시 라이브러리 오류를 발생하는 이유로, 텍스트 버전 이용.
            var version = AppSettings.GetVersion();
            string title = productName + " " + version;
            if (CurrentSequence != null)
            {
                title += " - " + (SequenceModified ? ModifiedMark : "") + CurrentSequence.FilePath;
            }
            return title;
        }

        internal string GetCompany()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
            return company;
        }

        /// <summary>
        /// 설정을 보관한다.
        /// </summary>
        internal void SaveAppSettings()
        {
            AppSettings.SharedInstance.Save();
            Settings.Default.Save();
        }

        /// <summary>
        /// 스텝을 새로 만들어 시퀀스에 추가한다.
        /// </summary>
        /// <param name="stepInfo"></param>
        /// <returns></returns>
        internal List<EolStep> InsertNewSteps(int index, object stepInfo, int count)
        {
            if (CurrentSequence != null && stepInfo is StepCategoryInfo categoryInfo && count > 0)
            {
                // 새로 생성되는 스텝의 ID는 이전 스텝의 ID+1.
                int newStepId;
                if (index < 1)
                {
                    newStepId = 0;
                }
                else
                {
                    newStepId = OriginalSteps[index - 1].StepId + 1;
                }

                // 유일 ID 할당을 위한 처리.
                int maxId = 0;
                if (OriginalSteps.Count > 0)
                {
                    maxId = OriginalSteps.Select(step => step.Id).Max();
                }

                var newSteps = new List<EolStep>();
                for (int i = 0; i < count; i++)
                {
                    var step = EolStep.CreateStep(categoryInfo.Category);

                    // ID 유일 할당.
                    step.Id = maxId + 1;
                    maxId += 1;

                    // 해당 StepId를 가진 스텝이 있으면, 중복되지 않을 때까지 1씩 증가.
                    while (true)
                    {
                        var stepExists = OriginalSteps.Where(s => s.StepId == newStepId).Any();
                        if (!stepExists)
                        {
                            break;
                        }

                        newStepId++;
                    }

                    step.No = index;
                    step.StepId = newStepId;
                    OriginalSteps.Insert(index, step);
                    newSteps.Add(step);

                    index++;
                }

                // 스텝 No 업데이트.
                UpdateStepNo();

                SequenceModified = true;
                return newSteps;
            }

            return null;
        }

        // Step NO 업데이트.
        private void UpdateStepNo()
        {
            if (OriginalSteps == null)
            {
                return;
            }

            for (int i = 0; i < OriginalSteps.Count; i++)
            {
                OriginalSteps[i].No = i + 1;
            }
        }

        /// <summary>
        /// 지정한 스텝들을 삭제한다.
        /// </summary>
        /// <param name="steps"></param>
        /// <returns></returns>
        internal bool RemoveSteps(IList steps)
        {
            if (CurrentSequence == null)
            {
                return false;
            }

            bool removed = false;
            foreach (var step in steps)
            {
                if (OriginalSteps.Remove(step as EolStep))
                {
                    removed = true;
                }
            }

            if (removed)
            {
                SequenceModified = true;
            }

            // Step NO 업데이트.
            UpdateStepNo();

            return removed;
        }

        /// <summary>
        /// 스텝들을 실행한다.
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="variant"></param>
        /// <param name="skipDisabled"></param>
        /// <param name="elozTestSet"></param>
        /// <param name="preAction"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        internal List<EolStep.TestResult> RunTestSteps(IEnumerable steps, string variant, bool skipDisabled, object elozTestSet, bool runCleanup,
            CancellationToken token, Action<EolStep> preAction, Action<EolStep, float> postAction)
        {
            var results = new List<EolStep.TestResult>();
            try
            {
                var testSteps = steps.Cast<EolStep>().ToList();
                results = RunSteps(testSteps, variant, skipDisabled, elozTestSet, token, preAction, postAction, out bool completed);

                // 시퀀스 설정에 따라 Cleanup 스텝 실행.
                // 시퀀스를 다 실행하지 못하고 중도에서 끝냈을 때만 Cleanup 스텝들을 실행.
                if (runCleanup && !completed && CurrentSequence.RunCleanup && CurrentSequence.CleanupSteps?.Length > 0)
                {
                    var cleanupSteps = new List<EolStep>();
                    foreach (var stepId in CurrentSequence.CleanupSteps)
                    {
                        var foundStep = FindStepById(testSteps, stepId);

                        // Cleanup 스텝을 찾지 못했더라도 에러를 발생시키지 않는다.
                        if (foundStep != null)
                        {
                            cleanupSteps.Add(foundStep);
                        }
                    }

                    if (cleanupSteps.Count > 0)
                    {
                        var cleanupResults = RunSteps(cleanupSteps, variant, skipDisabled, elozTestSet, token, preAction, null, out _);
                        results.AddRange(cleanupResults);
                    }
                }

                // 테스트가 끝나면 eloZ1 Relay를 끈다.
                if (elozTestSet != null)
                {
                    ElozDevice.SharedInstance.UnstimulateAll(elozTestSet, 0.1);
                    ElozDevice.SharedInstance.RelayOff(elozTestSet);
                }

                // 로그파일 생성.
                if (AppSettings.SharedInstance.LogEnabled)
                {
                    try
                    {
                        CreateLogFiles(results, Path.GetFileNameWithoutExtension(CurrentSequence.FilePath));
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Log file creation error: " + ex.Message);
                    }
                }

                // CAN 로그 파일 이름 변경.
                var logFilePath = Path.Combine(AppSettings.SharedInstance.LogFolderPath, PeakCanDevice.LogFileName);
                if (File.Exists(logFilePath))
                {
                    try
                    {
                        var fileName = "CAN_LOG_" + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss") + ".log";
                        var filePath = Path.Combine(AppSettings.SharedInstance.LogFolderPath, fileName);
                        File.Move(logFilePath, filePath);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("CAN log creation error: " + ex.Message);
                    }
                }

                TestDevice.CloseAllDevices();
                TestFinished?.Invoke(this, results);

                return results;
            }
            catch
            {
                TestDevice.CloseAllDevices();

                // 테스트가 끝나면 eloZ1 Relay를 끈다.
                if (elozTestSet != null)
                {
                    ElozDevice.SharedInstance.UnstimulateAll(elozTestSet, 0.1);
                    ElozDevice.SharedInstance.RelayOff(elozTestSet);
                }

                // 에러가 발생해도 테스트 종료 이벤트 호출.
                TestFinished?.Invoke(this, results);

                throw;
            }
        }

        private static List<EolStep.TestResult> RunSteps(List<EolStep> testSteps, string variant, bool skipDisabled, object elozTestSet,
            CancellationToken token, Action<EolStep> preAction, Action<EolStep, float> postAction, out bool completed)
        {
            // 마스터 보드이면 AbortOnFail 이 True여도 테스트를 중지하지 않는다.
            bool? isMasterGood = GetIsMasterBoard();

            // 진행 퍼센트 계산을 위해 실행할 스텝 개수 계산.
            int stepCount = 0;
            foreach (var step in testSteps)
            {
                // 이 스텝이 실행되는지 체크.
                bool shouldExecuteThisStep = true;
                if (skipDisabled && !step.Enabled)
                {
                    shouldExecuteThisStep = false;
                }
                else if (!step.CheckExecuteMode(isMasterGood))
                {
                    shouldExecuteThisStep = false;
                }

                // Variant에 따라 스텝을 실행해야 하는지 체크.
                if (shouldExecuteThisStep && !step.CheckVariant(variant))
                {
                    shouldExecuteThisStep = false;
                }

                if (shouldExecuteThisStep)
                {
                    stepCount++;
                }

                // Abort스텝 Jump횟수 초기화.
                if (step is EolAbortStep abortStep)
                {
                    abortStep.JumpedCount = 0;
                }
            }
            int stepNumber = 0;

            bool groupStarted = false;
            var results = new List<EolStep.TestResult>();
            int currentStepIndex;
            for (currentStepIndex = 0; currentStepIndex < testSteps.Count; currentStepIndex++)
            {
                token.ThrowIfCancellationRequested();

                var currentStep = testSteps[currentStepIndex];

                // 이 스텝이 실행되는지 체크.
                bool shouldExecuteThisStep = true;
                if (skipDisabled && !currentStep.Enabled)
                {
                    shouldExecuteThisStep = false;
                }
                else if (!currentStep.CheckExecuteMode(isMasterGood))
                {
                    shouldExecuteThisStep = false;
                }

                // Variant에 따라 스텝을 실행해야 하는지 체크.
                if (shouldExecuteThisStep && !currentStep.CheckVariant(variant))
                {
                    shouldExecuteThisStep = false;
                }

                if (!shouldExecuteThisStep)
                {
                    continue;
                }

                // 스텝 실행 전 UI 업데이트 등의 전처리.
                preAction?.Invoke(currentStep);

                // 그룹 시작 여부.
                if (currentStep.ElozRelayAfter == EolStep.ElozRelayMode.ON)
                {
                    groupStarted = true;
                }
                else if (groupStarted)
                {
                    groupStarted = false;
                    if (elozTestSet != null)
                    {
                        ElozDevice.SharedInstance.RelayOff(elozTestSet);
                    }
                }

                // 스텝 실행.
                EolStep.TestResult result;
                bool shouldAbort = false;
                bool shouldJump = false;
                EolStep jumpTarget = null;
                var abortRunSteps = new List<EolStep>();   // 중지할 때 실행해야 할 스텝 리스트.
                if (currentStep is EolAbortStep abortStep)
                {
                    // 실행 시간 측정.
                    var startTime = DateTime.Now;
                    var stopwatch = Stopwatch.StartNew();

                    // 실행 결과 초기화.
                    result = new EolStep.TestResult(abortStep)
                    {
                        ResultState = EolStep.ResultState.Pass,
                        ResultInfo = null,
                        ResultValue = null,
                        ResultValueState = EolStep.ResultValueState.Invalid,
                        Unit = Model.PhysicalUnit.None,
                    };

                    // 실행 전 Delay.
                    if (abortStep.DelayBefore > 0)
                    {
                        MultimediaTimer.Delay(abortStep.DelayBefore, token).Wait(token);
                    }

                    // Abort 조건에 따라 현재 실행을 중지할지 결정.
                    switch (abortStep.AbortCondition)
                    {
                        case EolAbortStep.AbortMode.AbortOnStepFailure:
                            // 앞선 테스트 결과들중 FAIL이 있는가 검사.
                            var totalResult = GetTotalResultState(results);
                            if (totalResult == EolStep.ResultState.Fail || totalResult == EolStep.ResultState.Aborted)
                            {
                                shouldAbort = true;
                            }
                            break;
                        case EolAbortStep.AbortMode.AbortOnPreviousStepFailure:
                            // 바로 앞 테스트 결과가 FAIL인지 검사.
                            if (results.Count > 0)
                            {
                                var lastResult = results[results.Count - 1];
                                var lastState = lastResult.ResultState;
                                if (!lastResult.Step.IgnoreResult)
                                {
                                    shouldAbort = lastState == EolStep.ResultState.Fail || lastState == EolStep.ResultState.Aborted;
                                }
                            }
                            break;
                        case EolAbortStep.AbortMode.AbortOnSpecifiedStepFailure:
                            // 지정한 스텝부터의 테스트 결과들중 FAIL이 있는가 검사.
                            if (results.Count > 0)
                            {
                                // 마지막 스텝부터 거꾸로 올라가면서 스텝 Id 비교.
                                var foundResult = results.First();
                                for (int i = results.Count - 1; i >= 0; i--)
                                {
                                    if (results[i].Step.Id == abortStep.AbortOnSpecifiedStepId)
                                    {
                                        foundResult = results[i];
                                        break;
                                    }
                                }

                                int foundResultIndex = results.IndexOf(foundResult);
                                totalResult = GetTotalResultState(results.GetRange(foundResultIndex, results.Count - foundResultIndex));
                                if (totalResult == EolStep.ResultState.Fail || totalResult == EolStep.ResultState.Aborted)
                                {
                                    shouldAbort = true;
                                }
                            }
                            break;
                        case EolAbortStep.AbortMode.AbortAlways:
                            shouldAbort = true;
                            break;
                        default:
                            shouldAbort = true;
                            break;
                    }

                    // Abort 할 때 실행해야 할 스텝 리스트.
                    if (shouldAbort && abortStep.Action == EolAbortStep.AbortAction.RunAndAbort)
                    {
                        if (abortStep.StepsToRun != null)
                        {
                            foreach (var id in abortStep.StepsToRun)
                            {
                                var foundStep = FindStepById(testSteps, id);
                                if (foundStep == null)
                                {
                                    result.ResultState = EolStep.ResultState.Fail;
                                    result.ResultInfo = $"Id={id}을 가진 스텝을 찾을 수 없습니다.";
                                    break;
                                }

                                abortRunSteps.Add(foundStep);
                            }
                        }
                    }
                    else if (shouldAbort && abortStep.Action == EolAbortStep.AbortAction.JumpTo)
                    {
                        shouldJump = true;

                        if (abortStep.JumpedCount < abortStep.JumpMaxCount)
                        {
                            // Jump할 타깃 스텝 찾기.
                            jumpTarget = FindStepById(testSteps, abortStep.JumpPosition);
                            if (jumpTarget == null)
                            {
                                result.ResultState = EolStep.ResultState.Fail;
                                result.ResultInfo = $"Id={abortStep.JumpPosition}을 가진 스텝을 찾을 수 없습니다.";
                            }
                            else
                            {
                                abortStep.JumpedCount += 1;
                            }
                        }
                    }

                    // eloZ1 Relay OFF.
                    if (elozTestSet != null && abortStep.ElozRelayAfter == EolStep.ElozRelayMode.OFF)
                    {
                        abortStep.RelayOff(elozTestSet);
                    }

                    stopwatch.Stop();
                    result.TotalMilliseconds = stopwatch.ElapsedMilliseconds;
                    result.FinishTime = DateTime.Now;
                    result.StartTime = startTime;
                    abortStep.RunResult = result;
                }
                else
                {
                    result = currentStep.Execute(elozTestSet, false, token);

                    // 만일 스텝이 Oscope스텝이고 Measure/DigitalVoltmeter 메서드라면 설정에 따라 Fail인 경우 다시 시도.
                    var currentOscopeStep = currentStep as EolOscopeStep;
                    bool isOscopeMeasurement = currentOscopeStep != null && currentOscopeStep.TestMethod == EolOscopeStep.OscopeTestMode.Measure;
                    if (isOscopeMeasurement && currentStep.RetestMode == EolStep.TestRetryMode.OnFail && result.ResultState == EolStep.ResultState.Fail)
                    {
                        // 위로 올라가면서 Capture를 찾고, Capture부터 현 스텝까지 Oscope 스텝들 다시 실행.
                        int captureStepIndex = -1;
                        for (int searchIndex = currentStepIndex; searchIndex >= 0; searchIndex--)
                        {
                            var searchStep = testSteps[searchIndex];
                            if (searchStep is EolOscopeStep searchOscopeStep && (!skipDisabled || searchOscopeStep.Enabled)
                                && searchOscopeStep.TestMethod == EolOscopeStep.OscopeTestMode.Capture
                                && currentOscopeStep.DeviceName == searchOscopeStep.DeviceName
                                && (searchOscopeStep.CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH1)
                                && currentOscopeStep.MeasurementSettings.Source1 == OscopeDevice.OscopeChannel.Channel1
                                || searchOscopeStep.CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH2)
                                && currentOscopeStep.MeasurementSettings.Source1 == OscopeDevice.OscopeChannel.Channel2
                                || searchOscopeStep.CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH3)
                                && currentOscopeStep.MeasurementSettings.Source1 == OscopeDevice.OscopeChannel.Channel3
                                || searchOscopeStep.CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH4)
                                && currentOscopeStep.MeasurementSettings.Source1 == OscopeDevice.OscopeChannel.Channel4))
                            {
                                // 검색조건: Oscope Capture 스텝, Enabled, 채널 같을것.
                                captureStepIndex = searchIndex;
                                break;
                            }
                        }

                        if (captureStepIndex >= 0)
                        {
                            // 현 Oscope 측정 스텝과 같은 채널을 캡쳐하는 Oscope 스텝을 위로 올라가면서 찾았으면, 거기서부터 다시 실행.
                            int retriedCount = 0;
                            while (retriedCount < currentStep.RetestCount)
                            {
                                // Cancel여부 체크.
                                token.ThrowIfCancellationRequested();

                                if (currentStep.RetestDelay > 0)
                                {
                                    MultimediaTimer.Delay(currentStep.RetestDelay, token).Wait(token);
                                }

                                // 스텝 실행 전 UI 업데이트 등의 전처리.
                                preAction?.Invoke(testSteps[captureStepIndex]);

                                // Oscope 캡쳐 스텝 실행.
                                var oscopeResult = testSteps[captureStepIndex].Execute(elozTestSet, false, token);
                                if (oscopeResult.ResultState == EolStep.ResultState.Aborted)
                                {
                                    break;
                                }
                                else if (oscopeResult.ResultState != EolStep.ResultState.Pass)
                                {
                                    retriedCount++;
                                    continue;
                                }

                                for (int runIndex = captureStepIndex + 1; runIndex <= currentStepIndex; runIndex++)
                                {
                                    // Oscope 측정 스텝들 다시 실행.
                                    var runningStep = testSteps[runIndex];
                                    var runningOscopeStep = runningStep as EolOscopeStep;
                                    isOscopeMeasurement = runningOscopeStep != null && runningOscopeStep.TestMethod == EolOscopeStep.OscopeTestMode.Measure;
                                    if (isOscopeMeasurement && (!skipDisabled || runningOscopeStep.Enabled)
                                        && runningOscopeStep.DeviceName == currentOscopeStep.DeviceName
                                        && runningOscopeStep.MeasurementSettings.Source1 == currentOscopeStep.MeasurementSettings.Source1)
                                    {
                                        // 스텝 실행 전 UI 업데이트 등의 전처리.
                                        preAction?.Invoke(runningOscopeStep);

                                        // 측정 스텝 실행.
                                        if (runIndex != currentStepIndex)
                                        {
                                            oscopeResult = runningOscopeStep.Execute(elozTestSet, false, token);
                                            if (oscopeResult.ResultState != EolStep.ResultState.Pass)
                                            {
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            result = runningOscopeStep.Execute(elozTestSet, false, token);
                                        }
                                    }
                                }

                                // 중간 측정 스텝이 Abort 이면 다시 테스트 종료.
                                if (oscopeResult.ResultState == EolStep.ResultState.Aborted)
                                {
                                    break;
                                }

                                // 현 스텝 실행 결과가 Aborted 또는 Pass 이면 다시 테스트 종료.
                                if (result.ResultState == EolStep.ResultState.Aborted || result.ResultState == EolStep.ResultState.Pass)
                                {
                                    break;
                                }

                                retriedCount++;
                            }
                        }
                    }
                }

                // 스텝 실행 후 UI 업데이트 등의 후처리.
                stepNumber++;
                float percent = (float)stepNumber / stepCount;
                if (percent > 100)
                {
                    percent = 100;
                }
                postAction?.Invoke(currentStep, percent);

                // 실행 결과 저장.
                results.Add(result);

                // 스텝 실행이 끝났음을 알려주기 위한 이벤트 호출.
                TestStepRunFinished?.Invoke(result);

                // Abort 스텝 실행.
                if (shouldJump && jumpTarget != null)
                {
                    // 타깃 스텝 위치 찾기.
                    int targetStepIndex = testSteps.IndexOf(jumpTarget);

                    // 현재 스텝부터 타깃 스텝까지 실행결과 초기화.
                    for (int stepIndex = currentStepIndex; stepIndex >= targetStepIndex; stepIndex--)
                    {
                        for (int resultIndex = results.Count - 1; resultIndex >= 0; resultIndex--)
                        {
                            if (results[resultIndex].Step == testSteps[stepIndex])
                            {
                                results.RemoveAt(resultIndex);
                                testSteps[stepIndex].RunResult = null;
                            }
                        }
                    }

                    // 진행 퍼센트 계산을 위한 처리.
                    // 현재 스텝부터 점프할 스텝까지 Enabled 설정에 따른 스텝 개수 계산.
                    var minStepIndex = Math.Min(currentStepIndex, targetStepIndex);
                    var maxStepIndex = Math.Max(currentStepIndex, targetStepIndex);
                    int jumpStepCount = 0;
                    for (int stepIndex = minStepIndex; stepIndex <= maxStepIndex; stepIndex++)
                    {
                        if (!skipDisabled || testSteps[stepIndex].Enabled)
                        {
                            jumpStepCount++;
                        }
                    }

                    if (targetStepIndex <= currentStepIndex)
                    {
                        stepNumber -= jumpStepCount;
                    }
                    else
                    {
                        jumpStepCount--;
                        if (!skipDisabled || testSteps[targetStepIndex].Enabled)
                        {
                            jumpStepCount--;
                        }
                        stepNumber += jumpStepCount;
                    }

                    // 실행 위치를 점프 타깃으로 이동.
                    currentStepIndex = targetStepIndex - 1;
                }
                else if (isMasterGood == null && shouldAbort && !shouldJump || result.ResultState == EolStep.ResultState.Aborted)
                {
                    // AbortOnFail 조건이 충족되거나, 스텝 결과가 Aborted이면 더이상 다른 스텝들 실행 안함.
                    if (result.ResultState == EolStep.ResultState.Pass && abortRunSteps.Count > 0)
                    {
                        // Abort 하기 전 실행해야 할 스텝 리스트.
                        var abortRunResults = RunSteps(abortRunSteps, variant, skipDisabled, elozTestSet, token, preAction, null, out _);
                        results.AddRange(abortRunResults);

                        // Debugging.
                        //Logger.LogVerbose($"Step {currentStep.Id}, {currentStep.Name} abort steps has been executed");
                    }

                    break;
                }

                // 테스트 설정에서 테스트 결과가 FAIL일 때 테스트를 중지하도록 설정했는지 체크.
                if (isMasterGood == null && currentStep.AbortOnFail && result.ResultState == EolStep.ResultState.Fail && !currentStep.IgnoreResult)
                {
                    break;
                }

                // AbortOnFail스텝의 JumpTo 횟수만큼 시도 후 FAIL이면 AbortOnFail 프로퍼티가 True일 때 테스트 중지.
                if (isMasterGood == null && currentStep.AbortOnFail && shouldJump && jumpTarget == null)
                {
                    break;
                }
            }

            completed = currentStepIndex >= testSteps.Count;
            return results;
        }

        /// <summary>
        /// 지정한 ID를 가진 스텝을 현재 로딩한 시퀀스에서 찾는다.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static EolStep FindStepById(List<EolStep> steps, int id)
        {
            if (steps == null)
            {
                return null;
            }

            return steps.Find(step => step.Id == id);
        }

        /// <summary>
        /// 테스트 결과를 지운다.
        /// </summary>
        internal void ClearTestResults()
        {
            if (CurrentSequence == null)
            {
                return;
            }

            foreach (var step in CurrentSequence.Steps)
            {
                step.ClearRunResult();
            }
        }

        /// <summary>
        /// 테스트가 실행중이라는 것을 표시하는 메시지와 그 배경색을 리턴한다.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="backColor"></param>
        internal static void GetRunningTextColor(out string message, out Color backColor)
        {
            message = "RUNNING";
            backColor = Color.Yellow;
        }

        /// <summary>
        /// 여러개의 테스트 결과를 종합적으로 평가하여 최종 테스트 결과를 도출, 그 메시지와 색깔을 리턴한다.
        /// </summary>
        /// <param name="runResults"></param>
        internal static EolStep.ResultState GetTotalResultState(IEnumerable<EolStep.TestResult> runResults, bool considerIgnoreResult = true)
        {
            var state = EolStep.ResultState.NoState;
            if (runResults != null)
            {
                foreach (var runResult in runResults)
                {
                    if (considerIgnoreResult && runResult.Step.IgnoreResult)
                    {
                        continue;
                    }

                    switch (runResult.ResultState)
                    {
                        case EolStep.ResultState.Pass:
                            if (state == EolStep.ResultState.NoState)
                            {
                                state = EolStep.ResultState.Pass;
                            }
                            break;
                        case EolStep.ResultState.Fail:
                            if (state != EolStep.ResultState.Aborted)
                            {
                                state = EolStep.ResultState.Fail;
                            }
                            break;
                        case EolStep.ResultState.Aborted:
                            state = EolStep.ResultState.Aborted;
                            break;
                    }
                }
            }
            return state;
        }

        /// <summary>
        /// 테스트 상태를 표시하는 텍스트와 배경색, 전경색을 리턴한다.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="stateText"></param>
        /// <param name="stateBackColor"></param>
        /// <param name="stateForeColor"></param>
        internal static void GetStateInfo(EolStep.ResultState state, out string stateText, out Color stateBackColor, out Color stateForeColor)
        {
            stateText = state.GetText();
            state.GetColors(out stateBackColor, out stateForeColor);
        }

        /// <summary>
        /// 스텝들을 Enable/Disable 시킨다.
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="enabled"></param>
        internal void SetStepsEnabled(IList steps, bool enabled)
        {
            foreach (var obj in steps)
            {
                if (obj is EolStep step)
                {
                    step.Enabled = enabled;
                }
            }
        }

        /// <summary>
        /// 스텝들에 Disabled 스텝이 있는가를 리턴한다.
        /// </summary>
        /// <param name="steps"></param>
        /// <returns></returns>
        internal bool ContainsDisabledStep(IList steps)
        {
            foreach (var obj in steps)
            {
                if (obj is EolStep step && !step.Enabled)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 스텝들을 Copy한다.
        /// </summary>
        /// <param name="steps"></param>
        internal void CopySteps(IList steps)
        {
            CopiedSteps.Clear();
            foreach (var step in steps)
            {
                var copiedStep = (step as EolStep)?.Clone();
                CopiedSteps.Add(copiedStep as EolStep);
            }
            Move = false;
        }

        /// <summary>
        /// 스텝들을 Cut한다.
        /// </summary>
        /// <param name="steps"></param>
        internal void CutSteps(IList steps)
        {
            CopiedSteps.Clear();
            foreach (var step in steps)
            {
                CopiedSteps.Add(step as EolStep);
            }
            Move = true;
        }

        /// <summary>
        /// Copy/Cut한 스텝들을 Paste한다.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="currentStep"></param>
        internal List<EolStep> PasteSteps(InsertPosition pos, object currentStep, out int insertedPosition)
        {
            var pastedSteps = new List<EolStep>();

            if (CopiedSteps.Count == 0)
            {
                insertedPosition = -1;
                return pastedSteps;
            }

            if (pos == InsertPosition.Current && currentStep == null)
            {
                pos = InsertPosition.Bottom;
            }

            // 유일 ID 생성.
            int maxId = 0;
            if (OriginalSteps.Count > 0)
            {
                maxId = OriginalSteps.Select(step => step.Id).Max();
            }

            if (!Move)
            {
                // Copy & Paste.
                int insertPos;
                switch (pos)
                {
                    case InsertPosition.Top:
                        insertPos = 0;
                        break;
                    case InsertPosition.Current:
                        insertPos = OriginalSteps.IndexOf(currentStep as EolStep) + 1;
                        break;
                    case InsertPosition.Bottom:
                    default:
                        insertPos = OriginalSteps.Count;
                        break;
                }

                insertedPosition = insertPos;

                foreach (var step in CopiedSteps)
                {
                    var cloneStep = step.Clone() as EolStep;
                    cloneStep.Id = maxId + 1;
                    maxId += 1;

                    OriginalSteps.Insert(insertPos, cloneStep);
                    pastedSteps.Add(cloneStep);
                    insertPos++;
                }
            }
            else
            {
                // Cut & Paste.

                // Cut하려는 스텝들 사이에 Current가 있는지 체크.
                EolStep insertPosStep = null;
                if (pos == InsertPosition.Current)
                {
                    if (CopiedSteps.Contains(currentStep))
                    {
                        // CurrentStep보다 위에 있고 CopiedSteps에 포함되지 않은 첫 스텝을 찾는다.
                        int currentIndex = OriginalSteps.IndexOf(currentStep as EolStep);
                        for (int i = currentIndex - 1; i >= 0; i--)
                        {
                            if (!CopiedSteps.Contains(OriginalSteps[i]))
                            {
                                insertPosStep = OriginalSteps[i];
                                break;
                            }
                        }
                    }
                    else
                    {
                        insertPosStep = currentStep as EolStep;
                    }
                }

                if (pos == InsertPosition.Current && insertPosStep == null)
                {
                    pos = InsertPosition.Top;
                }

                // 스텝 제거.
                foreach (var step in CopiedSteps)
                {
                    OriginalSteps.Remove(step);
                }

                // 스텝 삽입.
                int insertPos;
                switch (pos)
                {
                    case InsertPosition.Top:
                        insertPos = 0;
                        break;
                    case InsertPosition.Current:
                        insertPos = OriginalSteps.IndexOf(insertPosStep) + 1;
                        break;
                    case InsertPosition.Bottom:
                    default:
                        insertPos = OriginalSteps.Count;
                        break;
                }

                insertedPosition = insertPos;

                foreach (var step in CopiedSteps)
                {
                    OriginalSteps.Insert(insertPos, step);
                    pastedSteps.Add(step);
                    insertPos++;
                }

                // 이동한 후에는 CopiedSteps 초기화.
                CopiedSteps.Clear();
                Move = false;
            }

            // Step NO 업데이트.
            UpdateStepNo();

            return pastedSteps;
        }

        /// <summary>
        /// 클립보드에 스텝들을 저장한다.
        /// </summary>
        /// <param name="steps"></param>
        internal static void SetClipboard(IList steps)
        {
            var eolSteps = new List<EolStep>();
            foreach (var step in steps)
            {
                if (step is EolStep eolStep)
                {
                    eolSteps.Add(eolStep);
                }
            }

            try
            {
                var tempSequence = new EolSequence();
                tempSequence.Steps = eolSteps;
                var xmlText = tempSequence.ToXML();
                Clipboard.SetDataObject(xmlText, true);
            }
            catch (Exception ex)
            {
                Logger.LogError("Cannot copy steps to clipboard: " + ex.Message);
            }
        }

        /// <summary>
        /// 클립보드에 저장된 스텝 리스트를 얻는다.
        /// </summary>
        /// <returns></returns>
        internal static List<EolStep> GetClipboard()
        {
            try
            {
                var data = Clipboard.GetDataObject();
                if (data.GetDataPresent(typeof(string)))
                {
                    var xmlText = data.GetData(typeof(string)) as string;
                    if (string.IsNullOrEmpty(xmlText))
                    {
                        return null;
                    }

                    var sequence = EolSequence.LoadFromXML(xmlText);
                    return sequence.Steps;
                }
            }
            catch
            {
            }

            return null;
        }


        /// <summary>
        /// Copy/Cut한 스텝들을 Paste한다.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="currentStep"></param>
        internal List<EolStep> PasteClipboard(InsertPosition pos, object currentStep, out int insertedPosition)
        {
            var pastedSteps = new List<EolStep>();

            var clipboardData = GetClipboard();
            if (clipboardData == null || clipboardData.Count == 0)
            {
                insertedPosition = -1;
                return pastedSteps;
            }

            if (pos == InsertPosition.Current && currentStep == null)
            {
                pos = InsertPosition.Bottom;
            }

            // Paste.
            int insertPos;
            switch (pos)
            {
                case InsertPosition.Top:
                    insertPos = 0;
                    break;
                case InsertPosition.Current:
                    insertPos = OriginalSteps.IndexOf(currentStep as EolStep) + 1;
                    break;
                case InsertPosition.Bottom:
                default:
                    insertPos = OriginalSteps.Count;
                    break;
            }

            insertedPosition = insertPos;

            // 유일 ID 생성.
            int maxId = 0;
            if (OriginalSteps.Count > 0)
            {
                maxId = OriginalSteps.Select(step => step.Id).Max();
            }

            foreach (var step in clipboardData)
            {
                var cloneStep = step.Clone() as EolStep;
                cloneStep.Id = maxId + 1;
                maxId += 1;

                OriginalSteps.Insert(insertPos, cloneStep);
                pastedSteps.Add(cloneStep);
                insertPos++;
            }

            // Update Step NO.
            UpdateStepNo();

            return pastedSteps;
        }

        /// <summary>
        /// 지정한 스텝부터 마지막 스텝까지 입력한 시작 ID로부터 하나씩 증가하여 ID를 설정한다.
        /// </summary>
        /// <param name="modelObjects"></param>
        internal void RestartStepId(IList modelObjects)
        {
            if (modelObjects.Count > 0 && modelObjects[0] is EolStep startStep)
            {
                // 사용자에게 시작 번호를 물어본다.
                var dlg = new StepIdForm();
                dlg.Id = startStep.StepId;
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                int startId = dlg.Id;
                foreach (var modelObject in modelObjects)
                {
                    var step = modelObject as EolStep;
                    if (startId > 0)
                    {
                        step.StepId = startId;
                        startId++;
                    }
                    else
                    {
                        step.StepId = Math.Abs(startId);
                    }
                }
            }
        }

        /// <summary>
        /// 지정한 스텝들의 프로퍼티를 변경한다.
        /// </summary>
        /// <param name="modelObjects"><paramref name="propertyName"/>으로 지정한 Property 값들을 변경하려는 인스턴스.</param>
        /// <param name="propertyName">Property[.SubProperty[.SubSubProperty]] 형식의 Dot Notation 지원.</param>
        /// <param name="value"><paramref name="propertyName"/>으로 지정한 Property의 설정하려는 값.</param>
        internal void ChangeProperties(IList modelObjects, string propertyName, string value)
        {
            if (modelObjects.Count == 0 || string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            var propertyNames = propertyName.Split('.');
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase;
            foreach (var modelObject in modelObjects)
            {
                if (modelObject == null)
                {
                    continue;
                }

                // Property 찾기.
                // Property.SubProperty.SubSubProperty 형식의 dot notation 가능.
                object instance = modelObject;
                PropertyInfo property = null;
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if (i > 0)
                    {
                        instance = property.GetValue(instance);
                        if (instance == null)
                        {
                            break;
                        }
                    }

                    property = instance.GetType().GetProperty(propertyNames[i], flags);
                    if (property == null)
                    {
                        break;
                    }
                }

                if (property == null)
                {
                    continue;
                }

                // Property 값 변경.
                var converter = TypeDescriptor.GetConverter(property.PropertyType);
                if (converter != null)
                {
                    try
                    {
                        var convertedValue = converter.ConvertFrom(value);
                        property.SetValue(instance, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning($"Cannot change {propertyName} to {value}: {ex.Message}");
                    }
                }
            }

            //switch (propertyName)
            //{
            //    case nameof(EolStep.Section):
            //        foreach (var modelObject in modelObjects)
            //        {
            //            var step = modelObject as EolStep;
            //            step.Section = value;
            //        }
            //        break;
            //    case nameof(EolStep.Name):
            //        foreach (var modelObject in modelObjects)
            //        {
            //            var step = modelObject as EolStep;
            //            step.Name = value;
            //        }
            //        break;
            //    case nameof(EolStep.Remarks):
            //        foreach (var modelObject in modelObjects)
            //        {
            //            var step = modelObject as EolStep;
            //            step.Remarks = value;
            //        }
            //        break;
            //}
        }

        internal static bool GetCellBackColor(string propertyName, object modelObject, out Color backColor)
        {
            if (modelObject is EolStep step)
            {
                switch (propertyName)
                {
                    case nameof(EolStep.ResultStateDesc):
                        if (step.RunResult != null && !step.IgnoreResult)
                        {
                            step.RunResult.ResultState.GetColors(out backColor, out _);
                            return true;
                        }
                        break;
                }
            }

            backColor = Color.White;
            return false;
        }

        /// <summary>
        /// 로그 파일들을 만든다.
        /// </summary>
        /// <param name="results"></param>
        public static void CreateLogFiles(IEnumerable<EolStep.TestResult> results, string eolFileName)
        {
            // 파일이름 생성.
            var fileNameFormat = AppSettings.SharedInstance.LogFileNameFormat;
            if (string.IsNullOrWhiteSpace(fileNameFormat))
            {
                fileNameFormat = AppSettings.DefaultLogFileNameFormat;
            }
            fileNameFormat = fileNameFormat.Replace("{EOLFileName", "{0");
            fileNameFormat = fileNameFormat.Replace("{Date", "{1");
            var fileName = string.Format(fileNameFormat, eolFileName, DateTime.Now);

            // 결과 종합.
            var totalResult = GetTotalResultState(results);
            var stateText = totalResult == EolStep.ResultState.Pass ? "OK" : "NG";
            fileName = fileName + "_" + stateText;

            // 로그 텍스트 만들기.
            var fullLog = CreateFullLog(results);
            int resultCount = 0;
            long totalTime = 0;
            var specLog = CreateEOLSpecLog(true, results, ref resultCount, ref totalTime);

            // 로그파일 폴더 생성.
            var logFolderPath = AppSettings.SharedInstance.LogFolderPath;
            Directory.CreateDirectory(logFolderPath);

            // 로그파일 생성.
            File.WriteAllText(Path.Combine(logFolderPath, fileName + ".txt"), fullLog.ToString());
            File.WriteAllText(Path.Combine(logFolderPath, fileName + "_spec.csv"), specLog.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// 실행 결과 전체를 로그 텍스트로 만든다.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static StringBuilder CreateFullLog(IEnumerable<EolStep.TestResult> results)
        {
            var logBuilder = new StringBuilder();
            if (results == null)
            {
                return logBuilder;
            }

            const string fieldSeparator = ", ";

            foreach (var result in results)
            {
                // 시작시간.
                var startTime = result.StartTime.ToString("[yyyy-MM-dd HH:mm:ss.fff]");
                logBuilder.Append(startTime);

                // 결과.
                logBuilder.Append(" " + result.ResultState);

                // 결과 값.
                logBuilder.Append(fieldSeparator + result.ResultValue);

                // 단위.
                logBuilder.Append(fieldSeparator + result.Unit.GetText());

                // 스텝 이름.
                logBuilder.Append(fieldSeparator + result.Step.Name);

                // 디바이스 이름.
                logBuilder.AppendLine(fieldSeparator + result.Step.DeviceName);

                // 로그 바디.
                if (!string.IsNullOrEmpty(result.ResultLogBody))
                {
                    logBuilder.AppendLine(result.ResultLogBody);
                }
            }

            return logBuilder;
        }

        /// <summary>
        /// EOL 사양서 비교를 위한 로그를 생성한다.
        /// </summary>
        /// <param name="addHeader">CSV 헤더 생성 여부.</param>
        /// <param name="ictResult">ICT 실행결과.</param>
        /// <param name="resultCount">테스트 번호. 이 번호를 증가시키며 테스트 스텝 결과 추가.</param>
        /// <param name="totalMilliseconds">테스트 총 시간. 이 시간을 증가시키며 테스트 스텝 결과 추가.</param>
        /// <returns></returns>
        public static StringBuilder CreateEOLSpecLog(bool addHeader, TestResult ictResult, ref int resultCount, ref long totalMilliseconds)
        {
            var logBuilder = new StringBuilder();
            if (addHeader)
            {
                logBuilder.AppendLine(GetEOLSpecLogHeader());
            }

            if (ictResult == null)
            {
                return logBuilder;
            }

            // ICT 로그 중 EOL Spec 로그에 저장할 항목들만 저장.
            for (int recordIndex = 0; recordIndex < ictResult.TestRunResult.Count; recordIndex++)
            {
                TestResultRecord record = ictResult.TestRunResult[recordIndex];
                var stepResult = record as TestStepResultRecord;
                if (stepResult == null)
                {
                    continue;
                }

                for (int itemIndex = 0; itemIndex < stepResult.Count; itemIndex++)
                {
                    TestStepResultRecord.ItemTestResultRecord itemResult = stepResult[itemIndex];

                    // 스텝 이름이 EOL Spec 로그 패턴으로 시작하는지 검사.
                    if (!itemResult.TestStepName.StartsWith(EOLSpecLogPattern, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // 전체 시간.
                    totalMilliseconds += (long)((itemResult.RunTime ?? 0) * 1000);

                    // No.
                    resultCount++;
                    logBuilder.Append(resultCount);

                    // Name.
                    string stepName = itemResult.TestStepName.Substring(EOLSpecLogPattern.Length).Trim();
                    if (stepName?.Contains(specLogFieldSeparator) ?? false)
                    {
                        if (stepName.Contains("\""))
                        {
                            stepName = stepName.Replace("\"", "\"\"");
                        }
                        stepName = "\"" + stepName + "\"";
                    }
                    logBuilder.Append(specLogFieldSeparator + stepName);

                    // Min, Nominal, Max, Value.

                    var nominalValue = itemResult.ResultNominalValue;
                    var upperLimit = itemResult.ResultValueUpperLimit;
                    var lowerLimit = itemResult.ResultValueLowerLimit;

                    // 단위를 통일시킨다.
                    var unit = PhysicalUnitExtensions.From(itemResult.ResultValueUnit);
                    EolStep.GetPrefixExpression(nominalValue, unit, out MetricPrefix prefix);
                    var multiplier = prefix.GetMultiplier();
                    var unitText = prefix.GetText() + unit.GetText();

                    logBuilder.Append(specLogFieldSeparator + $"{lowerLimit / multiplier:0.####}");
                    //logBuilder.Append(specLogFieldSeparator + $"{nominalValue / multiplier:0.####}");
                    logBuilder.Append(specLogFieldSeparator + $"{upperLimit / multiplier:0.####}");
                    logBuilder.Append(specLogFieldSeparator + $"{itemResult.ResultValue / multiplier:0.####}");

                    // Unit.
                    logBuilder.Append(specLogFieldSeparator + unitText);

                    // Result.
                    bool passed = itemResult.ResultState == ResultState.Pass || itemResult.ResultState == ResultState.NoState;
                    logBuilder.Append(specLogFieldSeparator + (passed ? "OK" : "NG"));

                    // Test time.
                    //logBuilder.Append(specLogFieldSeparator + totalMilliseconds);

                    logBuilder.AppendLine();
                }
            }

            return logBuilder;
        }

        /// <summary>
        /// EOL 사양서 비교를 위한 로그를 생성한다.
        /// </summary>
        /// <param name="addHeader">CSV 헤더 생성 여부.</param>
        /// <param name="results">EOL 실행결과.</param>
        /// <param name="resultCount">테스트 번호. 이 번호를 증가시키며 테스트 스텝 결과 추가.</param>
        /// <param name="totalMilliseconds">테스트 총 시간. 이 시간을 증가시키며 테스트 스텝 결과 추가.</param>
        /// <returns></returns>
        public static StringBuilder CreateEOLSpecLog(bool addHeader, IEnumerable<EolStep.TestResult> results, ref int resultCount, ref long totalMilliseconds)
        {
            var logBuilder = new StringBuilder();
            if (addHeader)
            {
                logBuilder.AppendLine(GetEOLSpecLogHeader());
            }

            if (results == null)
            {
                return logBuilder;
            }

            // EOL 로그 중 EOL Spec 로그 용 스텝들만 저장.
            foreach (var result in results)
            {
                // 전체 시간.
                totalMilliseconds += result.TotalMilliseconds;

                // 스펙 검사용 로그에 저장하도록 설정된 스텝만 로그에 추가한다.
                if (!result.Step.ResultSpecLog)
                {
                    continue;
                }

                // No.
                resultCount++;
                logBuilder.Append(resultCount);

                // Name.
                string stepName = result.Step.Name;
                if (stepName?.Contains(specLogFieldSeparator) ?? false)
                {
                    if (stepName.Contains("\""))
                    {
                        stepName = stepName.Replace("\"", "\"\"");
                    }
                    stepName = "\"" + stepName + "\"";
                }
                logBuilder.Append(specLogFieldSeparator + stepName);

                // Min, Nominal, Max, Value.

                result.Step.GetNominalValues(out double? nominalValue, out double? upperLimit, out double? lowerLimit);

                // 단위를 통일시킨다.
                EolStep.GetPrefixExpression(nominalValue, result.Unit, out MetricPrefix prefix);
                var multiplier = prefix.GetMultiplier();
                var unitText = prefix.GetText() + result.Unit.GetText();

                if (nominalValue != null)
                {
                    // double값 표시.
                    logBuilder.Append(specLogFieldSeparator + $"{lowerLimit / multiplier:0.####}");
                    //logBuilder.Append(specLogFieldSeparator + $"{nominalValue / multiplier:0.####}");
                    logBuilder.Append(specLogFieldSeparator + $"{upperLimit / multiplier:0.####}");
                    logBuilder.Append(specLogFieldSeparator + $"{result.ResultValue / multiplier:0.####}");
                }
                else
                {
                    // CAN 등 측정값이 아닌 텍스트 표시 가능.
                    logBuilder.Append(specLogFieldSeparator + result.Step.MinValueText);
                    //logBuilder.Append(specLogFieldSeparator + result.Step.ExpectedValueDesc);
                    logBuilder.Append(specLogFieldSeparator + result.Step.MaxValueText);
                    logBuilder.Append(specLogFieldSeparator + result.Step.MeasuredValueDesc);
                }

                // Unit.
                logBuilder.Append(specLogFieldSeparator + unitText);

                // Result.
                //logBuilder.Append(specLogFieldSeparator + result.ResultState.GetText());
                bool passed = result.ResultState == EolStep.ResultState.Pass || result.ResultState == EolStep.ResultState.NoState;
                logBuilder.Append(specLogFieldSeparator + (passed ? "OK" : "NG"));

                // Test time.
                //logBuilder.Append(specLogFieldSeparator + totalMilliseconds);

                logBuilder.AppendLine();
            }

            return logBuilder;
        }

        private static string GetEOLSpecLogHeader()
        {
            var textBuilder = new StringBuilder();
            textBuilder.Append("Test");
            textBuilder.Append(specLogFieldSeparator + "Description");
            textBuilder.Append(specLogFieldSeparator + "Min");
            //textBuilder.Append(specLogFieldSeparator + "Nominal");
            textBuilder.Append(specLogFieldSeparator + "Max");
            textBuilder.Append(specLogFieldSeparator + "Value");
            textBuilder.Append(specLogFieldSeparator + "Unit");
            textBuilder.AppendLine(specLogFieldSeparator + "Result");
            //textBuilder.Append(specLogFieldSeparator + "Time(ms)");
            textBuilder.Append(",,,,,,");
            return textBuilder.ToString();
        }

        // 스텝이 해당 패턴을 포함하는지 검사한다.
        // PropertyName1=Value1 && Word1 || PropertyName2=Value2 && Word2
        private bool Match(EolStep step, string pattern, bool ignoreCase)
        {
            if (step == null || string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            // 검사하려는 전체 데이터를 하나의 문자열로 만들고 검사.
            var fullText = step.StepId + step.Section + step.Name + step.Remarks + step.ToleranceMinusDesc + step.ExpectedValueDesc +
                step.TolerancePlusDesc + step.DeviceName + step.TestModeDesc;
            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            // pattern을 OR(" || ")로 분리.
            var orTokens = pattern.Split(new string[] { " || " }, StringSplitOptions.RemoveEmptyEntries);
            bool orTokenMatch = false;
            foreach (string orToken in orTokens)
            {
                // OR token을 AND(" && ")로 분리.
                var andTokens = orToken.Split(new string[] { " && " }, StringSplitOptions.RemoveEmptyEntries);
                bool andTokenMatch = true;
                foreach (string andToken in andTokens)
                {
                    // PropertyName=Value 형식인지 체크.
                    var nameValuePair = andToken.Split('=');
                    if (nameValuePair.Length == 2)
                    {
                        // Property 값 비교.
                        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
                        if (ignoreCase)
                        {
                            flags |= BindingFlags.IgnoreCase;
                        }

                        // Property 찾기.
                        // Property.SubProperty.SubSubProperty 형식의 dot notation 가능.
                        var propertyNames = nameValuePair[0].Split('.');
                        object instance = step;
                        PropertyInfo property = instance.GetType().GetProperty(propertyNames[0], flags);
                        for (int i = 1; i < propertyNames.Length; i++)
                        {
                            if (property == null)
                            {
                                break;
                            }

                            instance = property.GetValue(instance);
                            if (instance == null)
                            {
                                break;
                            }

                            property = instance.GetType().GetProperty(propertyNames[i], flags);
                            if (property == null)
                            {
                                break;
                            }
                        }

                        if (property != null && instance != null)
                        {
                            // 값 비교.
                            var value = property.GetValue(instance);
                            var valueText = value?.ToString();
                            var startsWithAsterisk = nameValuePair[1].StartsWith("*");
                            var endsWithAsterisk = nameValuePair[1].EndsWith("*");
                            var asteriskTrimmed = nameValuePair[1].Trim('*');
                            if (startsWithAsterisk && endsWithAsterisk)
                            {
                                // 프로퍼티 값이 비교하려는 값을 포함하는지 체크.
                                andTokenMatch = valueText?.IndexOf(asteriskTrimmed, comparison) >= 0;
                            }
                            else if (startsWithAsterisk)
                            {
                                // 프로퍼티 값이 비교하려는 값으로 끝나는지 체크.
                                andTokenMatch = valueText?.EndsWith(asteriskTrimmed, comparison) ?? false;
                            }
                            else if (endsWithAsterisk)
                            {
                                // 프로퍼티 값이 비교하려는 값으로 시작하는지 체크.
                                andTokenMatch = valueText?.StartsWith(asteriskTrimmed, comparison) ?? false;
                            }
                            else
                            {
                                // 프로퍼티 값이 비교하려는 값과 일치하는지 체크.
                                andTokenMatch = string.Equals(nameValuePair[1], value?.ToString(), comparison);
                            }
                        }
                        else
                        {
                            // 프로퍼티가 없으면 false.
                            andTokenMatch = false;
                        }
                    }
                    else
                    {
                        // Word를 포함하는지 체크.
                        andTokenMatch = fullText.IndexOf(andToken, comparison) >= 0;
                    }

                    // AND token 중 하나만 false이면 전체 평가는 false로 되므로, 나머지 체크는 할 필요가 없음.
                    if (!andTokenMatch)
                    {
                        break;
                    }
                }

                // OR token 중 하나만 true이면 전체 평가는 true로 되므로, 나머지 체크는 할 필요가 없음.
                if (andTokenMatch)
                {
                    orTokenMatch = true;
                    break;
                }
            }

            return orTokenMatch;
        }

        /// <summary>
        /// 지정한 스텝 위치부터 앞 또는 뒤로 가면서 찾는다. 시퀀스의 맨 앞 또는 맨 뒤에 도달했는데도 못찾으면 null을 리턴한다.
        /// </summary>
        /// <param name="currentObj"></param>
        /// <param name="pattern"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        internal EolStep Find(object currentObj, string pattern, bool ignoreCase, bool forward, bool enabledOnly, bool specOnly)
        {
            if (string.IsNullOrEmpty(pattern) || OriginalSteps == null || OriginalSteps.Count == 0)
            {
                return null;
            }

            int startIndex;
            var currentStep = currentObj as EolStep;
            startIndex = OriginalSteps.IndexOf(currentStep);
            if (startIndex < 0)
            {
                if (forward)
                {
                    // 현재 선택된 스텝이 없으면 제일 앞에서부터 찾는다.
                    startIndex = -1;
                }
                else
                {
                    // 현재 선택된 스텝이 없으면 제일 뒤에서부터 찾는다.
                    startIndex = OriginalSteps.Count;
                }
            }

            if (forward)
            {
                // 시작위치부터 뒤로 가면서 찾는다.
                for (int stepIndex = startIndex + 1; stepIndex < OriginalSteps.Count; stepIndex++)
                {
                    var step = OriginalSteps[stepIndex];

                    // enabled된 스텝들만 찾는 경우를 체크.
                    if (enabledOnly && !step.Enabled)
                    {
                        continue;
                    }

                    // Spec에 보여주는 스텝들만 찾는 경우를 체크.
                    if (specOnly && !step.ResultSpecLog)
                    {
                        continue;
                    }

                    if (Match(step, pattern, ignoreCase))
                    {
                        return step;
                    }
                }
            }
            else
            {
                // 시작위치부터 앞으로 가면서 찾는다.
                for (int stepIndex = startIndex - 1; stepIndex >= 0; stepIndex--)
                {
                    var step = OriginalSteps[stepIndex];

                    // enabled된 스텝들만 찾는 경우를 체크.
                    if (enabledOnly && !step.Enabled)
                    {
                        continue;
                    }

                    if (Match(step, pattern, ignoreCase))
                    {
                        return step;
                    }
                }
            }

            return null;
        }

        internal static bool GetLicenseEnabled(string licenseKey)
        {
            try
            {
                return Common.LicenseManager.CheckLicenseKey(licenseKey);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Licensing Error: " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// 경과시간을 표시하는 텍스트를 만들어 리턴.
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        internal static string GetElapsedTimeText(long milliseconds)
        {
            return $"{milliseconds / 1000.0F:0.0}s";
        }

        /// <summary>
        /// 현재 시퀀스의 테스트 채널들을 지정한만큼 증가/감소 시킨 새로운 시퀀스를 만들어 리턴합니다.
        /// </summary>
        /// <param name="channelIncrement"></param>
        internal EolSequence CreateMultiPanelSequence(int channelIncrement)
        {
            if (CurrentSequence == null)
            {
                return null;
            }

            var clonedSequence = CurrentSequence.Clone() as EolSequence;
            clonedSequence.ModifyChannelNumbers(channelIncrement);
            return clonedSequence;
        }

        /// <summary>
        /// 시퀀스 파일의 변경 이력을 저장합니다.
        /// 변경 이력이 없으면 새로 만들고, 변경 이력이 있으면 새로 업데이트된 내용을 추가 저장합니다.
        /// </summary>
        /// <param name="isExternal">외부 프로그램에 의해 업데이트 되었는지 여부.</param>
        /// <param name="remarks">저장할 추가 설명.</param>
        internal void UpdateChangeHistory(bool isExternal, string remarks)
        {
            if (!File.Exists(FilePath))
            {
                return;
            }

            try
            {
                // 먼저 Change history를 로딩한다.
                var history = ChangeHistory.Load(FilePath);

                // Change history 파일이 없는 경우 히스토리 새로 생성.
                if (history == null)
                {
                    history = ChangeHistory.Create(FilePath);
                    if (isExternal)
                    {
                        history.UserName = "Windows";
                        history.UserRole = "Windows";
                    }
                    else if (CurrentUser != null)
                    {
                        history.UserName = CurrentUser.UserName;
                        history.UserRole = CurrentUser.Role.ToString();
                    }
                    history.Remarks = remarks;
                    history.Save(FilePath);
                    return;
                }

                // 이미 history가 있다면, 새로운 레코드 생성.
                var record = history.CreateRecord(FilePath);

                // 변경내용이 있으면 해당 내용 저장.
                if (record != null)
                {
                    if (isExternal)
                    {
                        record.UserName = "Windows";
                        record.UserRole = "Windows";
                    }
                    else if (CurrentUser != null)
                    {
                        record.UserName = CurrentUser.UserName;
                        record.UserRole = CurrentUser.Role.ToString();
                    }
                    record.Remarks = remarks;

                    if (history.HistoryRecords == null)
                    {
                        history.HistoryRecords = new List<ChangeRecord>();
                    }
                    history.HistoryRecords.Add(record);
                    history.Save(FilePath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"파일({FilePath}) 변경 이력 저장 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// Variant가 있는 스텝이 있으면 true를, 아니면 false를 리턴한다.
        /// </summary>
        /// <param name="steps"></param>
        /// <returns></returns>
        internal bool ContainsVariantStep(IList steps, object variant, bool? skipDisabled)
        {
            var boardVariant = variant as BoardVariant;
            foreach (var obj in steps)
            {
                if (obj is EolStep step && step.Variants?.Length > 0)
                {
                    if (skipDisabled == true && !step.Enabled)
                    {
                        continue;
                    }

                    if (boardVariant != null)
                    {
                        if (step.Variants.Contains(boardVariant.Name, StringComparer.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal BoardVariant CreateVariant()
        {
            return new BoardVariant();
        }

        /// <summary>
        /// 새로운 variant를 추가한다. 같은 variant가 이미 존재하면 추가하지 않고 false를 리턴한다.
        /// </summary>
        /// <param name="variant"></param>
        /// <returns>variant가 추가되었으면 true를, 아니면 false를 리턴한다.</returns>
        internal void AddVariant(BoardVariant variant)
        {
            if (CurrentSequence == null || variant == null)
            {
                return;
            }

            if (CurrentSequence.Variants == null)
            {
                CurrentSequence.Variants = new List<BoardVariant>() { variant };
            }
            else
            {
                CurrentSequence.Variants.Add(variant);
            }
            SequenceModified = true;
        }

        /// <summary>
        /// 지정한 variant와 같은 이름을 가진 첫번째 variant를 제거한다.
        /// </summary>
        /// <param name="variantObj"></param>
        /// <returns>variant를 제거했으면 true, 아니면 false를 리턴한다.</returns>
        internal bool RemoveVariant(object variantObj)
        {
            var variant = variantObj as BoardVariant;
            if (CurrentSequence == null || variant == null)
            {
                return false;
            }

            foreach (var oldVariant in CurrentSequence.Variants)
            {
                if (BoardVariant.Equals(oldVariant, variant))
                {
                    CurrentSequence.Variants.Remove(oldVariant);
                    SequenceModified = true;

                    // 스텝들에서 variant 제거.
                    foreach (var step in OriginalSteps)
                    {
                        if (step.Variants == null)
                        {
                            continue;
                        }

                        foreach (var variantName in step.Variants)
                        {
                            if (string.Equals(variantName, variant.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                var variantList = step.Variants.ToList();
                                variantList.Remove(variantName);
                                step.Variants = variantList.ToArray();
                                break;
                            }
                        }
                    }

                    break;
                }
            }

            return false;
        }
    }
}
