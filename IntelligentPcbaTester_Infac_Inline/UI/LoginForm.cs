using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            Text = Utils.AssemblyProduct;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            userNameTextBox.ImeMode = ImeMode.Alpha;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            // 사용자 이름이 비었는지 검사.
            if (string.IsNullOrEmpty(userNameTextBox.Text))
            {
                InformationBox.Show("사용자 이름을 입력하세요.", "입력 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                return;
            }

            // 사용자 이름, 암호가 맞는지 검사.
            UserManager userManager = AppSettings.GetUserManager();
            User foundUser = userManager.GetUser(userNameTextBox.Text, passwordTextBox.Text, out string errorMessage);
            if (foundUser == null)
            {
                InformationBox.Show(errorMessage, "로그인 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                return;
            }

            AppSettings.LoggedUser = foundUser;
            DialogResult = DialogResult.OK;
        }

        private void userNameTextBox_Enter(object sender, EventArgs e)
        {
            //Application.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo("en-US"));
            userNameTextBox.ImeMode = ImeMode.Alpha;
        }
    }
}
