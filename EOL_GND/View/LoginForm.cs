using EOL_GND.Model;
using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace EOL_GND.View
{
    public partial class LoginForm : Form
    {
        internal User LoggedUser { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            userNameTextBox.ImeMode = ImeMode.Alpha;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            // 사용자 이름이 비었는지 검사.
            if (string.IsNullOrWhiteSpace(userNameTextBox.Text))
            {
                InformationBox.Show("사용자 이름을 입력하세요.", "입력 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                userNameTextBox.Focus();
                userNameTextBox.SelectAll();
                return;
            }

            // 사용자가 있는지 찾는다.
            User foundUser = UserManager.SharedInstance.GetUser(userNameTextBox.Text);
            if (foundUser == null)
            {
                InformationBox.Show("존재하지 않는 사용자입니다.", "로그인 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                userNameTextBox.Focus();
                userNameTextBox.SelectAll();
                return;
            }

            // 사용자 암호를 검증한다.
            bool match = UserManager.VerifyPassword(foundUser.Password, foundUser.Salt, passwordTextBox.Text);
            if (!match)
            {
                InformationBox.Show("암호가 맞지 않습니다.", "로그인 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                passwordTextBox.Focus();
                passwordTextBox.SelectAll();
                return;
            }

            LoggedUser = foundUser;
            DialogResult = DialogResult.OK;
        }

        private void userNameTextBox_Enter(object sender, EventArgs e)
        {
            //Application.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo("en-US"));
            userNameTextBox.ImeMode = ImeMode.Alpha;
        }
    }
}
