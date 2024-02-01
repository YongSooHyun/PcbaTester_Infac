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

public class TestProgram
{
    private const string VarName_SelectedVariant = "SelectedVariant";
    private const string VarName_SelectedTestSection = "SelectedTestSection";

    public void Run(int ProcessID, TestProgramEnvironment TPEnv)
    {
        string test_variant;

        if (TestFramework.Common.GlobalStorage.Exists(VarName_SelectedVariant))
            test_variant = TestFramework.Common.GlobalStorage.Get(VarName_SelectedVariant).ToString();
        else
            test_variant = "";

        if (!string.IsNullOrEmpty(test_variant))
            TPEnv.SelectVariant(ProcessID, test_variant);   // make sure that test_variant contains a valid variant name, otherwise the test run will abort
        /*        else
                {
                    // global variant variable is not set -> we are in edit mode -> ask for the variant
                    string[] variant_names = new string[] {"MQ4","NQ5","CN7","TM","NX4","NX4C","SG2","T-CAR"}; // list all valid variant names here (as defined in board description)
                    TestFramework.Common.Dialog.SelectionBoxResult result;
                    result = TestFramework.Common.Dialog.SelectionBox.Show("No variant selected!\nSelect a variant:", "Variant Selection", variant_names);
                    if (result.IsOk)
                        TPEnv.SelectVariant(ProcessID, result.Text);
                    else
                        throw new System.Exception("No variant selected");
                }*/

        if (TestFramework.Common.GlobalStorage.Exists(VarName_SelectedTestSection))
        {
            string test_section = TestFramework.Common.GlobalStorage.Get(VarName_SelectedTestSection).ToString();
            TPEnv.SelectTestSequence(ProcessID, test_section);
            TPEnv.RunSelectedTests(ProcessID);
        }
        else
        {
            TPEnv.SelectAllTestSequences(ProcessID);
            TPEnv.RunSelectedTests(ProcessID);
        }

        TPEnv.TriggerErrorIndication(ProcessID);
    }

    public string Info()
    {
        return "";
    }
}
