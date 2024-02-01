using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class JtagSettingsForm : Form
    {
        public JtagSettingsForm()
        {
            InitializeComponent();

            useRadioButton.Checked = AppSettings.UseJtag;
            notUseRadioButton.Checked = !useRadioButton.Checked;
        }

        private void useRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.UseJtag = useRadioButton.Checked;
        }
    }
}
