using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class PasswordSettingsForm : Form
    {
        public PasswordSettingsForm()
        {
            InitializeComponent();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            userNameTextBox.Text = AppSettings.LoggedUser.UserName;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            var manager = AppSettings.GetUserManager();
            if (!manager.ChangePassword(userNameTextBox.Text, currentPasswordTextBox.Text, 
                newPasswordTextBox.Text, confirmPasswordTextBox.Text, out string errorMsg))
            {
                InformationBox.Show(errorMsg, "암호 변경 오류", icon: InformationBoxIcon.Error);
            }
            else
            {
                InformationBox.Show("변경되었습니다.", "암호 변경", icon: InformationBoxIcon.Success);
            }
        }
    }
}
