using EOL_GND.Common;
using EOL_GND.Model;
using EOL_GND.ViewModel;
using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.View
{
    public partial class MultiBoardForm : Form
    {
        internal SequenceViewModel ViewModel { get; set; }

        public MultiBoardForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Init();
        }

        private void Init()
        {
            if (ViewModel?.FilePath != null)
            {
                // 파일이름 생성. 기존 파일이름에 -n 붙여 만든다.
                // 이름속에 포함된 마지막 수를 찾는다.
                var pattern = @"(\d+)(?!.*\d)";
                var match = Regex.Match(ViewModel.FilePath, pattern);
                if (int.TryParse(match?.Value, out int parsed))
                {
                    filePathTextBox.Text = Regex.Replace(ViewModel.FilePath, pattern, (parsed + 1).ToString());
                }
                else
                {
                    var directory = Path.GetDirectoryName(ViewModel.FilePath);
                    var fileName = Path.GetFileNameWithoutExtension(ViewModel.FilePath);
                    var extension = Path.GetExtension(ViewModel.FilePath);
                    var filePath = Path.Combine(directory, fileName + "-2");
                    if (!string.IsNullOrEmpty(extension))
                    {
                        filePath += extension;
                    }
                    filePathTextBox.Text = filePath;
                }
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (var dialog = SequenceForm.CreateSaveFileDialog())
            {
                dialog.Title = "Save As";
                dialog.OverwritePrompt = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    filePathTextBox.Text = dialog.FileName;
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewModel?.SequenceOpened ?? false)
                {
                    int channelOffset = (int)chOffsetNUDown.Value;
                    var newSequence = ViewModel.CreateMultiPanelSequence(channelOffset);
                    newSequence.SaveAs(filePathTextBox.Text);

                    // 결과 보여주기.
                    InformationBox.Show($"A new sequence file has been created:{Environment.NewLine}{filePathTextBox.Text}",
                        "Multi Board Creation", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
                DialogResult = DialogResult.None;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {

        }
    }
}
