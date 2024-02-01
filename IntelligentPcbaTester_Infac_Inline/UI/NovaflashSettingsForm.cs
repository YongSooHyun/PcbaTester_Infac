using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class NovaflashSettingsForm : Form
    {
        public NovaflashSettingsForm()
        {
            InitializeComponent();

            lanRadioButton.Checked = AppSettings.NovaflashUseLan;
            ipTextBox.Text = AppSettings.NovaflashLanIp;
            comPortTextBox.Text = AppSettings.NovaflashSerialPortName;
            baudRateNumericUpDown.Maximum = int.MaxValue;
            baudRateNumericUpDown.Value = AppSettings.NovaflashSerialBaudRate;
        }

        private void lanRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.NovaflashUseLan = lanRadioButton.Checked;
            lanGroupBox.Enabled = lanRadioButton.Checked;
            serialGroupBox.Enabled = !lanRadioButton.Checked;
        }

        private void ipTextBox_TextChanged(object sender, EventArgs e)
        {
            AppSettings.NovaflashLanIp = ipTextBox.Text;
        }

        private void comPortTextBox_TextChanged(object sender, EventArgs e)
        {
            AppSettings.NovaflashSerialPortName = comPortTextBox.Text;
        }
    }
}
