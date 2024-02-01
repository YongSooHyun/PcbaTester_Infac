using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 사용자 리스트를 파일로 저장, 로딩 한다.
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// 보관 파일 이름.
        /// </summary>
        private static string FileName => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "login.cfg");

        /// <summary>
        /// 유저 리스트. 파일로부터 Serialize/Deserialize 된다.
        /// </summary>
        public List<User> Users { get; set; } = new List<User>();

        private UserManager()
        {
        }

        /// <summary>
        /// 이 클래스의 인스턴스를 파일에 보관한다.
        /// </summary>
        internal void Save()
        {
            using (var writer = new StreamWriter(FileName))
            {
                var xmlSerializer = new XmlSerializer(GetType(), GetType().Namespace);
                xmlSerializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// 이 클래스를 XML파일로부터 로딩한다.
        /// </summary>
        /// <returns>로딩한 오브젝트.</returns>
        internal static UserManager Load()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                var xmlSerializer = new XmlSerializer(typeof(UserManager), typeof(UserManager).Namespace);
                var obj = xmlSerializer.Deserialize(stream) as UserManager;
                return obj;
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(UserManager)}.{nameof(Load)}(): {e.Message}");
                var manager = new UserManager();

                // 디폴트 유저 추가.
                manager.Users.Add(new User
                {
                    UserName = "admin",
                    Password = "admin",
                    Role = UserRole.관리자,
                });

                return manager;
            }
            finally
            {
                stream?.Close();
            }
        }

        /// <summary>
        /// 사용자 이름과 암호를 검증하여 해당하는 유저를 리턴한다.
        /// </summary>
        /// <param name="userName">사용자 ID.</param>
        /// <param name="password">암호.</param>
        /// <param name="errorMessage">오류 메시지.</param>
        /// <returns>검증되었으면 <see cref="User"/> 인스턴스를, 아니면 null을 리턴한다.</returns>
        internal User GetUser(string userName, string password, out string errorMessage)
        {
            foreach (var user in Users)
            {
                if (user.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                {
                    if (user.Password.Equals(password, StringComparison.Ordinal))
                    {
                        errorMessage = "";
                        return user;
                    }
                    else
                    {
                        errorMessage = "암호가 맞지 않습니다.";
                        return null;
                    }
                }
            }

            errorMessage = "존재하지 않는 사용자입니다.";
            return null;
        }

        internal static bool ValidateValue(List<User> users, User currentUser, string propertyName, string propertyValue, out string errorMessage)
        {
            switch (propertyName)
            {
                case nameof(User.UserName):
                    if (string.IsNullOrEmpty(propertyValue))
                    {
                        errorMessage = "빈 문자열은 입력할 수 없습니다.";
                        return false;
                    }

                    // 사용자 이름은 유일해야 한다.
                    foreach (var user in users)
                    {
                        if (user == currentUser)
                        {
                            continue;
                        }

                        if (propertyValue.Equals(user.UserName, StringComparison.OrdinalIgnoreCase))
                        {
                            errorMessage = "이미 존재하는 사용자 이름입니다.";
                            return false;
                        }
                    }
                    break;
            }

            errorMessage = "";
            return true;
        }

        /// <summary>
        /// 현재 로그인한 유저의 사용자 이름과 암호를 변경한다.
        /// </summary>
        /// <param name="userName">변경하려는 사용자 이름.</param>
        /// <param name="currentPassword">현재 암호.</param>
        /// <param name="newPassword">바꾸려는 새 암호.</param>
        /// <param name="confirmPassword">새 암호 확인.</param>
        /// <param name="errorMessage">실패하는 경우 오류 메시지.</param>
        /// <returns>성공하면 true, 실패하면 false.</returns>
        internal bool ChangePassword(string userName, string currentPassword, string newPassword, string confirmPassword, out string errorMessage)
        {
            if (AppSettings.LoggedUser == null)
            {
                errorMessage = "로그인한 사용자 정보가 없습니다.";
                return false;
            }

            // 사용자 이름의 유일성 검증.
            foreach (var user in Users)
            {
                if (AppSettings.LoggedUser == user)
                {
                    continue;
                }

                if (userName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage = "이미 존재하는 사용자 이름입니다.";
                    return false;
                }
            }

            // 현재 암호 체크.
            string userPassword = AppSettings.LoggedUser.Password ?? "";
            if (!userPassword.Equals(currentPassword, StringComparison.Ordinal))
            {
                errorMessage = "현재 암호가 맞지 않습니다.";
                return false;
            }

            // 새 암호, 암호 확인이 일치하는지 체크.
            if (!newPassword.Equals(confirmPassword, StringComparison.Ordinal))
            {
                errorMessage = "새 암호와 암호 확인이 일치하지 않습니다.";
                return false;
            }

            string oldName = AppSettings.LoggedUser.UserName;
            string oldPassword = AppSettings.LoggedUser.Password;
            AppSettings.LoggedUser.UserName = userName;
            AppSettings.LoggedUser.Password = newPassword;
            
            try
            {
                Save();
            }
            catch (Exception e)
            {
                AppSettings.LoggedUser.UserName = oldName;
                AppSettings.LoggedUser.Password = oldPassword;
                errorMessage = $"사용자 정보 저장 오류: {e.Message}";
                return false;
            }

            errorMessage = "";
            return true;
        }
    }
}
