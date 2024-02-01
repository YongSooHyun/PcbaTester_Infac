using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using TestFramework.PluginTestCell;
using TestFramework.PluginTestCell.TestResults;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 테스트 환경 오류가 발생할 때 그 정보를 담고 있다.
    /// </summary>
    class ErrorOccurredEventArgs : EventArgs
    {
        internal int ErrorId { get; }
        internal string ErrorMessage { get; }
        internal string ErrorDetails { get; }

        internal ErrorOccurredEventArgs(int id, string message, string details)
        {
            ErrorId = id;
            ErrorMessage = message;
            ErrorDetails = details;
        }
    }

    /// <summary>
    /// 새로운 테스트 결과(스텝)에 대한 정보를 담고 있다.
    /// </summary>
    class NewTestResultEventArgs : EventArgs
    {
        internal TestResultRecord ResultRecord { get; }

        internal NewTestResultEventArgs(TestResultRecord record)
        {
            ResultRecord = record;
        }
    }

    /// <summary>
    /// 테스트 종료 시 그 결과에 대한 정보를 담고 있다.
    /// </summary>
    class TestRunFinishedEventArgs : EventArgs
    {
        internal ResultState FinishedState { get; }

        internal TestRunFinishedEventArgs(ResultState state)
        {
            FinishedState = state;
        }
    }

    /// <summary>
    /// eloZ1 디바이스와 통신을 진행한다.
    /// </summary>
    class Eloz1
    {
        /// <summary>
        /// 테스트 환경에서 오류가 발생하는 경우의 이벤트.
        /// </summary>
        internal event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>
        /// 테스트 실행 과정에 새로운 테스트 결과(스텝)가 발생하는 경우의 이벤트.
        /// </summary>
        internal event EventHandler<NewTestResultEventArgs> NewTestResult;

        /// <summary>
        /// 테스트가 종료될 때 발생하는 이벤트.
        /// </summary>
        internal event EventHandler<TestRunFinishedEventArgs> TestRunFinished;

        /// <summary>
        /// 테스트 실행 상태.
        /// </summary>
        internal bool Running { get; private set; } = false;

        /// <summary>
        /// 테스트 실행 환경.
        /// </summary>
        internal static TestCellEnvironment TestEnvironment { get; set; }

        // 테스트 환경으로부터 얻은 테스트 결과 오브젝트.
        internal TestResult TestResult { get; private set; }

        // eloZ1 Project 인스턴스.
        internal Project CurrentProject { get; private set; } = new Project();
        private string currentProjectName = "";
        private bool opened = false;
        private bool writeMode = false;

        // 실행할 섹션 이름을 저장하는 변수 이름.
        private const string SectionVarName = "SelectedTestSection";
        private const string VariantVarName = "SelectedVariant";

        internal Eloz1()
        {
            InitTestFramework();
        }

        // 테스트 프레임워크를 초기화한다.
        private void InitTestFramework()
        {
            if (TestEnvironment == null)
            {
                return;
            }

            // 오류 핸들러 설정.
            TestEnvironment.ErrorOccured += OnErrorOccurred;
            TestEnvironment.ErrorOccuredEventEnabled = true;
            //CurrentProject.ErrorOccurred += OnErrorOccurred;

            // 테스트 결과 이벤트 핸들러 설정.
            TestResult = TestEnvironment.GetTestResult();
            //TestResult = CurrentProject.GetTestResult();
            TestResult.TestRunFinished += OnTestRunFinished;
            TestResult.NewTestResultRecord += OnNewTestResultRecord;
            TestResult.ConnectToResultLog();
        }

        private void CloseTestFramework()
        {
            if (TestEnvironment == null)
            {
                return;
            }

            if (TestResult != null)
            {
                TestResult.DisconnectFromResultLog();
                //testCellEnvironment.ErrorOccured -= OnErrorOccurred;
                //testCellEnvironment.ErrorOccuredEventEnabled = false;
                TestResult.TestRunFinished -= OnTestRunFinished;
                TestResult.NewTestResultRecord -= OnNewTestResultRecord;
                TestResult.Dispose();
                TestResult = null;
            }

            TestEnvironment.ErrorOccured -= OnErrorOccurred;
            TestEnvironment.ErrorOccuredEventEnabled = false;
        }

        internal void Close()
        {
            //testCellEnvironment.ReturnMessage = "";
            if (opened)
            {
                CurrentProject.CloseProject();
                opened = false;
            }
            currentProjectName = "";
            CloseTestFramework();
        }

        internal void OpenProject(string projectName, bool writeAccess)
        {
            if (!projectName.Equals(currentProjectName, StringComparison.OrdinalIgnoreCase) || writeMode != writeAccess)
            {
                if (opened)
                {
                    CurrentProject.CloseProject();
                    opened = false;
                }
                opened = CurrentProject.OpenProject(projectName, writeAccess);

                // Workaround for 'TestFramework-Query-Bug'
                if (opened && (bool)CurrentProject.GetItem("TestData").GetProperty("#IsNull"))
                {
                    CurrentProject.CloseProject();
                    opened = false;
                    opened = CurrentProject.OpenProject(projectName, writeAccess);
                }
                // End of the workaround
            }
            else
            {
                opened = true;
            }

            if (!opened)
            {
                currentProjectName = "";
                throw new Exception($"'{projectName}' 프로젝트를 열 수 없습니다.");
            }

            currentProjectName = projectName;
            writeMode = writeAccess;
            CurrentProject.GeneralLogMode = Project.LogMode.LogAlways;
        }

        /// <summary>
        /// 지정한 프로젝트를 편집하는 TestBuilder Editor 를 보여준다.
        /// </summary>
        /// <param name="projectName"></param>
        internal void ShowTestEditor(string projectName)
        {
            // 프로젝트 열기.
            OpenProject(projectName, true);

            // Editor 보여주기.
            CurrentProject.ShowTestEditor();
        }

        /// <summary>
        /// 테스트를 실행한다.
        /// </summary>
        /// <param name="projectName">실행하려는 테스트 프로젝트 이름.</param>
        /// <returns></returns>
        internal void RunTest(string sectionName, string variantName)
        {
            Running = true;

            try
            {
                // 실행할 variant 설정.
                if (!string.IsNullOrEmpty(variantName))
                {
                    TestFramework.Common.GlobalStorage.Set(VariantVarName, variantName);
                }
                else if (TestFramework.Common.GlobalStorage.Exists(VariantVarName))
                {
                    TestFramework.Common.GlobalStorage.Remove(VariantVarName);
                }

                // 실행할 섹션 이름 설정.
                if (!string.IsNullOrEmpty(sectionName))
                {
                    TestFramework.Common.GlobalStorage.Set(SectionVarName, sectionName);
                }
                else if (TestFramework.Common.GlobalStorage.Exists(SectionVarName))
                {
                    TestFramework.Common.GlobalStorage.Remove(SectionVarName);
                }

                CurrentProject.RunTest();

                if (TestFramework.Common.GlobalStorage.Exists(VariantVarName))
                {
                    TestFramework.Common.GlobalStorage.Remove(VariantVarName);
                }
                if (TestFramework.Common.GlobalStorage.Exists(SectionVarName))
                {
                    TestFramework.Common.GlobalStorage.Remove(SectionVarName);
                }
            }
            finally
            {
                Running = false;
            }
        }

        internal void StopTest()
        {
            CurrentProject.BreakRunningTest();
        }

        protected void OnErrorOccurred(int id, string message, string details)
        {
            ErrorOccurred?.Invoke(this, new ErrorOccurredEventArgs(id, message, details));
        }

        private void OnNewTestResultRecord(TestFramework.PluginTestCell.TestResults.TestResultRecord testResultRecord)
        {
            NewTestResult?.Invoke(this, new NewTestResultEventArgs(testResultRecord));
        }

        private void OnTestRunFinished(TestFramework.PluginTestCell.TestResults.ResultState totalTestState)
        {
            TestRunFinished?.Invoke(this, new TestRunFinishedEventArgs(totalTestState));
        }

        /// <summary>
        /// Eloz1 에 Attach 된 프로젝트들 중에서 유저가 선택한 프로젝트 이름을 리턴한다.
        /// </summary>
        /// <param name="prompt">사용자에게 보여줄 문자열.</param>
        /// <param name="title">대화상자 제목.</param>
        internal static string SelectProject(string prompt, string title, string selectedItem = null)
        {
            string[] attached_projects;
            TestFramework.Common.Dialog.SelectionBoxResult ret;

            attached_projects = GetNamesOfAttachedProjects()?.OrderBy(s => s)?.ToArray();
            if (attached_projects != null)
            {
                ret = TestFramework.Common.Dialog.SelectionBox.Show(prompt, title, attached_projects, selectedItem);
                if (ret.IsOk)
                {
                    return ret.Text;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the names of all attached test projects
        /// </summary>
        /// <returns></returns>
        internal static string[] GetNamesOfAttachedProjects()
        {
            string[] project_names = null;
            object[] props;
            props = new Project().GetItem("TestFramework").GetProperty("AttachedProjects") as object[];
            if (props != null)
            {
                project_names = new string[props.Length];
                for (int i = 0; i < props.Length; i++)
                    project_names[i] = (string)props[i];
            }
            return project_names;
        }

        /// <summary>
        /// eloZ1 글로벌 사용 변수 설정 함수.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetGlobalStorageValue(string key, object value)
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

        /// <summary>
        /// eloZ1 글로벌 사용 변수 값 얻기 함수.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetGlobalStorageValue(string key)
        {
            object value = null;
            if (TestFramework.Common.GlobalStorage.Exists(key))
            {
                value = TestFramework.Common.GlobalStorage.Get(key);
            }
            return value;
        }
    }
}
