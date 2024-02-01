using EOL_GND.Device;
using EOL_GND.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.ViewModel
{
    internal class UserEditorViewModel
    {
        internal List<User> GetUsers()
        {
            return UserManager.SharedInstance.Users;
        }

        internal void Save()
        {
            UserManager.SharedInstance.Save();
        }

        internal User CreateUser()
        {
            var newName = UserManager.SharedInstance.GetNewUserName("NewUser");
            var newUser = UserManager.CreateUser(newName, string.Empty);
            UserManager.SharedInstance.Users.Add(newUser);
            return newUser;
        }

        internal void DeleteUsers(IList userObjects)
        {
            var users = UserManager.SharedInstance.Users;
            foreach (var userObject in userObjects)
            {
                users.Remove(userObject as User);
            }
        }

        internal void ConfigureEditingControl(string propertyName, Control editControl)
        {
            switch (propertyName)
            {
                case nameof(User.PlainPassword):
                    if (editControl is TextBox passwordTextBox)
                    {
                        passwordTextBox.UseSystemPasswordChar = true;
                    }
                    break;
            }

            if (editControl is TextBox textBox)
            {
                textBox.SelectAll();
            }
            else if (editControl is NumericUpDown nuDown)
            {
                nuDown.Select(0, nuDown.Text.Length);
            }
        }
    }
}
