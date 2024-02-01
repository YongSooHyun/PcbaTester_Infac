using EOL_GND.Common;
using EOL_GND.Model;
using EOL_GND.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.View
{
    public partial class SequenceOptionsForm : Form
    {
        internal SequenceViewModel ViewModel { get; set; }

        public SequenceOptionsForm()
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
            if (ViewModel?.SequenceOpened ?? false)
            {
                cleanupCheckBox.Checked = ViewModel.RunCleanup;
                cleanupTextBox.Enabled = ViewModel.RunCleanup;
                if (ViewModel.CleanupSteps != null)
                {
                    cleanupTextBox.Text = new Model.ComponentModel.IntArrayConverter().ConvertTo(ViewModel.CleanupSteps, typeof(string)).ToString();
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewModel?.SequenceOpened ?? false)
                {
                    // Cleanup 스텝 리스트 텍스트 파싱.
                    var converter = new Model.ComponentModel.IntArrayConverter();
                    var intArray = converter.ConvertFrom(cleanupTextBox.Text) as int[];

                    // 지정한 스텝 아이디가 모두 있는지 체크.
                    if (intArray != null)
                    {
                        var steps = ViewModel.OriginalSteps.ToList();
                        foreach (var id in intArray)
                        {
                            var foundStep = SequenceViewModel.FindStepById(steps, id);
                            if (foundStep == null)
                            {
                                throw new Exception($"Id={id}을 가진 스텝을 찾을 수 없습니다.");
                            }
                        }
                    }

                    ViewModel.RunCleanup = cleanupCheckBox.Checked;
                    ViewModel.CleanupSteps = intArray;

                    // Variants.
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

        private void cleanupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            cleanupTextBox.Enabled = cleanupCheckBox.Checked;
        }
    }
}
