using EOL_GND.Common;
using EOL_GND.Model;
using EOL_GND.ViewModel;
using InfoBox;
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
    public partial class RunOptionsForm : Form
    {
        internal SequenceViewModel ViewModel { get; set; }

        /// <summary>
        /// Disabled된 스텝들을 건너뛸지 여부.
        /// </summary>
        public bool SkipDisabled => !disabledCheckBox.Checked;

        /// <summary>
        /// 실행하려는 variant.
        /// </summary>
        public string Variant => variantComboBox.SelectedItem?.ToString();

        public RunOptionsForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ViewModel?.Variants?.Count > 0)
            {
                variantComboBox.Items.AddRange(ViewModel.Variants.Select(v => v.Name).ToArray());
            }
            else
            {
                variantGroupBox.Enabled = false;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (variantComboBox.Items.Count > 0 && variantComboBox.SelectedItem == null)
            {
                InformationBox.Show("실행하려는 Variant를 선택하세요.", title: "Variant 설정", icon: InformationBoxIcon.Warning);
                DialogResult = DialogResult.None;
            }
        }
    }
}
