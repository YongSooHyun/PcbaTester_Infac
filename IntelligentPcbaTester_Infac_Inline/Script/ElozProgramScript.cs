//==================================================
// Begin of code replaced by Multi Panel Wizard
//--------------------------------------------------
//
//using TestFramework.PluginTestProgram;
//public class TestProgram : TestFramework.PluginTestProgram.ITestProgramInterface
//{
//    public string Info()
//    {
//        return "Default test program";
//    }
//
//    public void Run(int ProcessID, TestProgramEnvironment TPEnv)
//    {
//        TPEnv.SelectAllTestSequences(ProcessID);
//        TPEnv.RunSelectedTests(ProcessID);
//
//        TPEnv.TriggerErrorIndication(ProcessID);
//    }
//}
//
//--------------------------------------------------
// End of code replaced by Multi Panel Wizard
//==================================================


/*==================================================
Test program script created by Multi Panel Wizard

Please note:

The board(s) to be tested can be selected.
The selection string is a comma separated list of board names (e.g. "A,B,E").
The string can be assigned to the GlobalStorage variable "SelectedBoards".
If the test program is executed and the variable is not available, an input
dialog will be opened for selection.

The value of the constant MaxNumberOfRecontacts defines the maximum number of recontacts.

The test section names are built by the following schema: <base name>(<board name>)
The base names of all discharge test sections are listed in DischargeTestSectionPatterns,
the base names of all contact test sections are listed in ContactTestSectionPatterns,
the base names of all shorts test sections are listed in ShortsTestSectionPatterns,
the base names of all remaining test sections are listed in TestSectionPatterns,
the name of the test section executing deferred tests is assigned to DeferredTestsSectionName,
all board names are listed in AllBoardNames.

The wizard has declared and initialized all these variables.
If a test sections or a board should be added later on,
the initializations need to be modified accordingly.

==================================================*/

using TestFramework.PluginTestProgram;
using TestFramework.Common;
using TestFramework.Script.Diagnostics;

public class TestProgram : TestFramework.PluginTestProgram.ITestProgramInterface
{
    const string SelectedBoardsVariable = "SelectedBoards";
    const int MaxNumberOfRecontacts = 1;
    const string AllBoardsSelection = "{ALL}";

    string[] DischargeTestSectionPatterns = new string[] { };
    string[] ContactTestSectionPatterns = new string[] { "ContactTest" };
    string[] ShortsTestSectionPatterns = new string[] { };
    string[] TestSectionPatterns = new string[] { "Tests" };
    string DeferredTestsSectionName = "DeferredTests";
    string AllBoardNames = "Board1,Board2,Board3,Board4";

    //--------------------------------------------------

    public string Info()
    {
        return "Multi panel test program";
    }

    //--------------------------------------------------

    public void Run(int ProcessID, TestProgramEnvironment TPEnv)
    {
        string[] TestSections = null;
        string[] BoardList = null;
        string SelectedBoards = "";
        Dialog.InputBoxResult DialogResult;

        if (GlobalStorage.Exists(SelectedBoardsVariable))
            SelectedBoards = GlobalStorage.Get(SelectedBoardsVariable) as string;
        else
        {
            DialogResult = Dialog.InputBox.Show(
                                            "Please enter the names of the boards to be tested (comma separated)." + "\n" +
                                            "To test all boards enter " + AllBoardsSelection + ".",
                                            "Board Selection",
                                            AllBoardsSelection);
            if ((DialogResult.IsOk))
                SelectedBoards = DialogResult.Text;
        }

        if (!string.IsNullOrEmpty(SelectedBoards))
        {
            if (SelectedBoards == AllBoardsSelection)
                SelectedBoards = AllBoardNames;
            BoardList = SelectedBoards.Split(new char[] { ',' });
            if (BoardList.Length > 0)
            {
                TestProgramEnvironment.ResultState Result = TestProgramEnvironment.ResultState.NoState;
                // Discharge
                TestSections = CreateTestSectionList(BoardList, DischargeTestSectionPatterns);
                if (TestSections.Length > 0)
                {
                    TPEnv.SelectTestSequences(ProcessID, TestSections);
                    TPEnv.RunSelectedTests(ProcessID);
                    Result = TPEnv.GetTestRunResultState(ProcessID);
                }
                if (Result == TestProgramEnvironment.ResultState.Pass | Result == TestProgramEnvironment.ResultState.NoState)
                {
                    // Contact Test
                    TestSections = CreateTestSectionList(BoardList, ContactTestSectionPatterns);
                    if (TestSections.Length > 0)
                    {
                        TPEnv.SelectTestSequences(ProcessID, TestSections);
                        TPEnv.RunSelectedTests(ProcessID);
                        Result = TPEnv.GetTestRunResultState(ProcessID);
                        if (Result == TestProgramEnvironment.ResultState.Fail)
                        {
                            // Recontact and then repeat Contact Test
                            for (int Count = 1; Count <= MaxNumberOfRecontacts; Count++)
                            {
                                TPEnv.RecontactFixture(ProcessID);
                                TPEnv.RerunSelectedTests(ProcessID);
                                if (Result == TestProgramEnvironment.ResultState.Pass)
                                    break;
                            }
                        }
                    }
                    if (Result == TestProgramEnvironment.ResultState.Pass | Result == TestProgramEnvironment.ResultState.NoState)
                    {
                        // Remaining Tests
                        TestSections = CreateTestSectionList(BoardList, ShortsTestSectionPatterns, TestSectionPatterns);
                        TPEnv.SelectTestSequences(ProcessID, TestSections);
                        TPEnv.RunSelectedTests(ProcessID);
                    }
                }
            }
            else
            {
                TPEnv.SelectAllTestSequences(ProcessID);
                TPEnv.RunSelectedTests(ProcessID);
            }
        }

        TPEnv.TriggerErrorIndication(ProcessID);
    }



    //--------------------------------------------------

    string[] CreateTestSectionList(string[] BoardList, string[] TestSectionPatterns)
    {
        return CreateTestSectionList(BoardList, null, TestSectionPatterns);
    }

    //--------------------------------------------------

    public string[] CreateTestSectionList(string[] BoardList, string[] ShortsTestSectionPatterns, string[] TestSectionPatterns)
    {
        string[] TestSectionList = new string[] { };
        int Index;
        int LastIndex;

        LastIndex = (BoardList.Length * TestSectionPatterns.Length) - 1;
        if (ShortsTestSectionPatterns != null)
        {
            if (ShortsTestSectionPatterns.Length > 0)
                LastIndex = LastIndex + (BoardList.Length * ShortsTestSectionPatterns.Length) + 1;
        }
        if (LastIndex >= 0)
        {
            Index = 0;
            TestSectionList = new string[LastIndex + 1];
            if (ShortsTestSectionPatterns != null)
            {
                if (ShortsTestSectionPatterns.Length > 0)
                {
                    foreach (string board in BoardList)
                    {
                        foreach (string SectionPattern in ShortsTestSectionPatterns)
                        {
                            TestSectionList[Index] = SectionPattern + "(" + board.Trim() + ")";
                            Index++;
                        }
                    }
                    TestSectionList[Index] = DeferredTestsSectionName;
                    Index++;
                }
            }
            foreach (string board in BoardList)
            {
                foreach (string SectionPattern in TestSectionPatterns)
                {
                    TestSectionList[Index] = SectionPattern + "(" + board.Trim() + ")";
                    Index++;
                }
            }
        }

        return TestSectionList;
    }
}
