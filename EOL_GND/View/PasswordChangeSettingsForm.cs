using EOL_GND.Common;
using EOL_GND.Model;
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
    public partial class PasswordChangeSettingsForm : Form
    {
        public PasswordChangeSettingsForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // 현재 로그인한 사용자 이름 표시.
            var currentUser = AppSettings.GetCurrentUser();
            userNameTextBox.Text = currentUser?.UserName;
            userNameTextBox.ReadOnly = true;
            Enabled = currentUser != null;
        }

        private void changeButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 로그인한 사용자가 있는지 체크.
                var currentUser = AppSettings.GetCurrentUser();
                if (currentUser == null)
                {
                    InformationBox.Show("현재 로그인한 사용자가 없습니다.", "Error", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                    return;
                }

                // 사용자 이름 체크.
                //if (string.IsNullOrWhiteSpace(userNameTextBox.Text))
                //{
                //    MessageBox.Show("사용자 이름을 입력하세요.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                // 현재 암호 체크.
                bool passwordMatch = UserManager.VerifyPassword(currentUser.Password, currentUser.Salt, currentPasswordTextBox.Text);
                if (!passwordMatch)
                {
                    InformationBox.Show("현재 암호가 맞지 않습니다.", "Error", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                    return;
                }

                // 새 암호, 암호 확인이 일치하는지 체크.
                if (!string.Equals(newPasswordTextBox.Text, confirmPasswordTextBox.Text, StringComparison.Ordinal))
                {
                    InformationBox.Show("새 암호와 암호 확인이 일치하지 않습니다.", "Error", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                    return;
                }

                // 암호 변경.
                UserManager.ChangePassword(currentUser, newPasswordTextBox.Text);
                UserManager.SharedInstance.Save();

                InformationBox.Show("암호가 변경되었습니다.", "암호 변경", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Success);
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
        }
    }
}
