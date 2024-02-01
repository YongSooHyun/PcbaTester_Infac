using EOL_GND.Common;
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
    public partial class LicenseForm : Form
    {
        public LicenseForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                requestCodeTextBox.Text = Common.LicenseManager.GetRequestCode();
                licenseKeyTextBox.Text = AppSettings.SharedInstance.LicenseKey;
            }
            catch (Exception ex)
            {
                Logger.LogError("Request code error: " + ex.Message);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var licenseKey = licenseKeyTextBox.Text;
            if (!SequenceViewModel.GetLicenseEnabled(licenseKey))
            {
                InformationBox.Show("Invalid license key.", "Error", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }

            AppSettings.SharedInstance.LicenseKey = licenseKey;
        }
    }
}
