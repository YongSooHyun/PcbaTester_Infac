using System.IO;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class PathSettingsForm : Form
    {
        public PathSettingsForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            Init();
        }

        private void Init()
        {
            // Project Path.
            projectPathTextBox.Text = AppSettings.ProjectFolder;

            // Novaflash Path.
            novaflashPathTextBox.Text = AppSettings.NovaflashFolder;

            // Log Path.
            logPathTextBox.Text = AppSettings.TestLogLocalFolder;
            mesLogPathTextBox.Text = AppSettings.TestLogMesFolder;
            otherLogPathTextBox.Text = AppSettings.TestLogOtherFolder;
            printLogFolderTextBox.Text = AppSettings.PrintLogFolder;

            // App Log File Path.
            appLogFilePathTextBox.Text = AppSettings.AppLogFilePath;

            // PCB Viewer.
            pcbViewerTextBox.Text = AppSettings.PcbViewerPath;
        }

        private void projectPathBrowseButton_Click(object sender, System.EventArgs e)
        {
            var path = SelectFolder(AppSettings.ProjectFolder);
            if (path != null)
            {
                projectPathTextBox.Text = path;
                AppSettings.ProjectFolder = path;
            }
        }

        private void novaflashPathBrowseButton_Click(object sender, System.EventArgs e)
        {
            var path = SelectFolder(AppSettings.NovaflashFolder);
            if (path != null)
            {
                novaflashPathTextBox.Text = path;
                AppSettings.NovaflashFolder = path;
            }
        }

        private void logPathBrowseButton_Click(object sender, System.EventArgs e)
        {
            var path = SelectFolder(AppSettings.TestLogLocalFolder);
            if (path != null)
            {
                logPathTextBox.Text = path;
                AppSettings.TestLogLocalFolder = path;
            }
        }

        private string SelectFolder(string initialPath)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = initialPath;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            else
            {
                return null;
            }
        }

        private void appLogPathBrowseButton_Click(object sender, System.EventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Log Files(*.log)|*.log;|All Files(*.*)|*.*;";
            dialog.InitialDirectory = Path.GetDirectoryName(AppSettings.AppLogFilePath);
            dialog.RestoreDirectory = true;
            dialog.CreatePrompt = true;
            dialog.OverwritePrompt = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                appLogFilePathTextBox.Text = dialog.FileName;
                AppSettings.AppLogFilePath = dialog.FileName;
            }
        }

        private void mesLogPathBrowseButton_Click(object sender, System.EventArgs e)
        {
            var path = SelectFolder(AppSettings.TestLogMesFolder);
            if (path != null)
            {
                mesLogPathTextBox.Text = path;
                AppSettings.TestLogMesFolder = path;
            }
        }

        private void otherLogPathBrowseButton_Click(object sender, System.EventArgs e)
        {
            var path = SelectFolder(AppSettings.TestLogOtherFolder);
            if (path != null)
            {
                otherLogPathTextBox.Text = path;
                AppSettings.TestLogOtherFolder = path;
            }
        }

        private void printLogFolderBrowseButton_Click(object sender, System.EventArgs e)
        {
            var path = SelectFolder(AppSettings.PrintLogFolder);
            if (path != null)
            {
                printLogFolderTextBox.Text = path;
                AppSettings.PrintLogFolder = path;
            }
        }

        private void mesLogPathTextBox_TextChanged(object sender, System.EventArgs e)
        {
            AppSettings.TestLogMesFolder = mesLogPathTextBox.Text;
        }

        private void pcbViewerBrowseButton_Click(object sender, System.EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Executable Files(*.exe)|*.exe;|All Files(*.*)|*.*;";
            dialog.RestoreDirectory = true;
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pcbViewerTextBox.Text = dialog.FileName;
                AppSettings.PcbViewerPath = dialog.FileName;
            }
        }
    }
}
