using IntelligentPcbaTester;
using System;
using TestFramework.PluginTestCell;

namespace TestFrameworkPlugin
{
    class TestCell : ITestCellInterface
    {
        public void Configure()
        {
            // Do nothing.
        }

        public string Info()
        {
            return Utils.AssemblyDescription;
        }

        public void Run(TestCellEnvironment environment, string command, string argument)
        {
            try
            {
                Eloz1.TestEnvironment = environment;
                var loginForm = new LoginForm();

                if (command.StartsWith("TFW."))
                {
                    if (command == "TFW.OpenProject")
                    {
                        if (loginForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            new MainForm().ShowDialog();
                        }
                    }
                }
                else
                {
                    if (loginForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        new MainForm().ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("eloZ1 Plugin Error: " + ex.Message);
            }
        }
    }
}
