using System;
using TestFramework.PluginTestMethod;
using TestFramework.TestDevices;
using TestFramework.VirtualTestSets.TestSet_A1;
using TestFramework.VirtualTestSets.TestSet_A1.Devices;

// EOL 파일 실행 스크립트.
public class TestMethod
{
    // 실행할 variant.
    // 스크립트에서 현재 실행되고 있는 variant를 얻는 방법이 현재 없으므로 수동으로 설정.
    private const string SelectedVariant = "";

    // Configuration of the TestSet (especially the MatrixSize should be adjusted):
    // eloZ1 DMM 모듈을 사용할 수 있는 경우: configString = "MatrixSize:1088, Stimulus10V, Stimulus60V, MeasurementUnit"
    // eloZ1 DMM 모듈을 사용할 수 없는 경우: configString = "MatrixSize:1088, Stimulus10V, Stimulus60V"
    private const string configString = "MatrixSize:1088, Stimulus10V, Stimulus60V, MeasurementUnit";

    // 실행하려는 파일 경로, 시작번호, 끝번호, 스텝 개수.
    // 스크립트 이름을 파싱해 얻으려고 했으나, 현재 불가능.
    // 파일 경로도 현재 프로젝트 경로의 최상위 폴더를 찾을 수 없음.
    // EOL_StepCount 는 최소 실행해야 할 스텝 개수로, 시작번호와 끝번호 사이에 있는
    // Enabled된 스텝 개수가 이보다 작으면 에러를 발생한다.
    // 시작번호, 끝번호가 0이면 검증 없이 모든 스텝 실행.
    private const string EOL_FileName = "D:\\ElozPlugin\\EOL\\EOL_Infac\\ICCU_FILTER_NA_V142.eol";
    private const int EOL_StartNumber = 1;
    private const int EOL_EndNumber = 20;
    private const int EOL_StepCount = 20;

    // 섹션 전체 실행 시 윈도우 자동 꺼짐 지연 시간(ms). 0 이하이면 자동으로 꺼지지 않음.
    private const int AutoCloseDelay = 500;

    public void Execute(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // insert code executing the method
        DebugClear();

        TestSet testSet = null;
        string resultInfo = "";
        TestStep.ResultState state = TestStep.ResultState.NoState;
        try
        {
            // eloZ1 Relay를 위한 디바이스 요청.
            testSet = TestSet.GetDevice(ProcessID, configString);

            // EOL 파일 실행(UI 표시함).
            bool autoStart = !ThisTestStep.IsDebugMode();
            EOL_GND.View.SequenceForm dlg = new EOL_GND.View.SequenceForm(EOL_FileName, testSet, autoStart,
                EOL_StartNumber, EOL_EndNumber, EOL_StepCount);
            dlg.AutoCloseDelay = AutoCloseDelay;
            dlg.SelectedVariant = SelectedVariant;
            dlg.ShowDialog();
            EOL_GND.Model.EolStep.ResultState eolResultState = dlg.AutoStartResult;

            // EOL 파일 실행 결과를 스크립트 결과로 설정할 수 있도록 변환.
            switch (eolResultState)
            {
                case EOL_GND.Model.EolStep.ResultState.Aborted:
                    state = TestStep.ResultState.Aborted;
                    break;
                case EOL_GND.Model.EolStep.ResultState.Fail:
                    state = TestStep.ResultState.Fail;
                    break;
                case EOL_GND.Model.EolStep.ResultState.Pass:
                    state = TestStep.ResultState.Pass;
                    break;
                case EOL_GND.Model.EolStep.ResultState.NoState:
                    state = TestStep.ResultState.Fail;
                    break;
            }
        }
        catch (Exception ex)
        {
            string errorMessage = "Error: " + ex.Message;
            DebugWriteLine(errorMessage);
            resultInfo = errorMessage;
            state = TestStep.ResultState.Aborted;
        }
        finally
        {
            if (testSet != null)
            {
                testSet.Dispose();
            }
        }

        // 이 스크립트의 실행 결과는 EOL 파일을 실행한 전체 결과에 따라 설정.
        ThisTestStep.SetResult(
            state,
            resultInfo,
            0,
            TestStep.PhysicalUnit.NoPhysicalUnit,
            TestStep.ResultValueState.Invalid);
    }

    public string Info()
    {
        return "";
    }

    public void Validate(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // insert code to validate the parameters
    }

    public void RequestResources(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // insert code to request resources
        TesterController.ResourceRequest(ProcessID, TesterController.EvaluationModes.EarlyEvaluation);
        TestSet.RequestResources(ProcessID, configString, TestSet.EvaluationMode.EarlyEvaluation);
    }

    public void Evaluate(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // insert code evaluating the results
    }

    internal static void DebugClear()
    {
        TestFramework.Script.Diagnostics.Debug.ClearOutputScreen();
    }

    internal static void DebugWriteLine(string message)
    {
        DateTime now = DateTime.Now;
        string timeString = string.Format("[{0:D2}:{1:D2}:{2:D2}.{3:D3}]", now.Hour, now.Minute, now.Second, now.Millisecond);
        TestFramework.Script.Diagnostics.Debug.WriteLine(timeString + " " + message);
    }
}
