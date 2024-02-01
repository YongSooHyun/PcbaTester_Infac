using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND_Setup_Actions
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            // Register PATH environment variable.
            var targetDir = Context.Parameters["TARGETDIR"];

            if (!string.IsNullOrEmpty(targetDir))
            {
                InstallActions.AddEnvironmentPathValue(targetDir.TrimEnd(Path.DirectorySeparatorChar));
            }
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);

            // Remove registered PATH environment variable.
            var targetDir = Context.Parameters["TARGETDIR"];

            if (!string.IsNullOrEmpty(targetDir))
            {
                InstallActions.RemoveEnvironmentPathValue(targetDir.TrimEnd(Path.DirectorySeparatorChar));
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            // Remove registered PATH environment variable.
            var targetDir = Context.Parameters["TARGETDIR"];

            if (!string.IsNullOrEmpty(targetDir))
            {
                InstallActions.RemoveEnvironmentPathValue(targetDir.TrimEnd(Path.DirectorySeparatorChar));
            }
        }
    }
}
