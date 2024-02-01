using BrightIdeasSoftware;
using EOL_GND.Common;
using EOL_GND.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    /// <summary>
    /// 사용자 리스트를 파일로 저장, 로딩 한다.
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// 보관 파일 이름.
        /// </summary>
        //private static string FileName => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "users.config");
        private const string FileName = "D:\\ElozPlugin\\eol_users.config";

        /// <summary>
        /// Shared instance.
        /// </summary>
        public static UserManager SharedInstance
        {
            get
            {
                if (_sharedInstance == null)
                {
                    _sharedInstance = Load();
                }
                return _sharedInstance;
            }
        }
        private static UserManager _sharedInstance = null;

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
                var xmlSerializer = new XmlSerializer(typeof(UserManager));
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
                var xmlSerializer = new XmlSerializer(typeof(UserManager));
                var obj = xmlSerializer.Deserialize(stream) as UserManager;
                if (obj.Users.Count == 0)
                {
                    obj.AddDefaultUser();
                }
                return obj;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot load users info: {ex.Message}");

                var manager = new UserManager();
                manager.AddDefaultUser();
                return manager;
            }
            finally
            {
                stream?.Close();
            }
        }

        /// <summary>
        /// 디폴트 유저 추가.
        /// </summary>
        internal void AddDefaultUser()
        {
            var userName = "ad";
            var password = "a";
            userName += "m";
            password += "dmi";
            userName += "in";
            password += "n";
            var newUser = CreateUser(userName, password);
            newUser.Role = UserRole.Administrator;
            Users.Add(newUser);
        }

        /// <summary>
        /// 지정한 사용자 이름과 암호를 가진 사용자를 만든다.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        internal static User CreateUser(string userName, string password)
        {
            // Create a salt.
            var rng = new RNGCryptoServiceProvider();
            var salt = new byte[67];
            rng.GetBytes(salt);

            // Hash a password.
            var keyDerivation = new Rfc2898DeriveBytes(password, salt, 2013, HashAlgorithmName.SHA512);
            var hash = keyDerivation.GetBytes(64);

            // Create a user.
            return new User
            {
                UserName = userName,
                Password = Convert.ToBase64String(hash),
                Salt = Convert.ToBase64String(salt)
            };
        }

        /// <summary>
        /// 패스워드가 매칭되는지 체크한다.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <param name="enteredPassword"></param>
        /// <returns></returns>
        internal static bool VerifyPassword(string password, string salt, string enteredPassword)
        {
            // 텍스트를 바이트 배열로 변환.
            var saltBytes = Convert.FromBase64String(salt);
            var passwordBytes = Convert.FromBase64String(password);

            // 입력한 암호의 해시값 계산.
            var keyDerivation = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 2013, HashAlgorithmName.SHA512);
            var hash = keyDerivation.GetBytes(64);

            return hash.SequenceEqual(passwordBytes);
        }

        /// <summary>
        /// 지정한 사용자 이름을 가진 유저를 리턴한다.
        /// </summary>
        /// <param name="userName">사용자 ID.</param>
        /// <returns><see cref="User"/> 인스턴스를 리턴한다.</returns>
        internal User GetUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return null;
            }

            return Users.Where(user => string.Equals(user.UserName, userName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// 사용자 이름이 유일한지 체크하여 결과를 리턴한다.
        /// </summary>
        /// <param name="users"></param>
        /// <param name="currentUser"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
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
        /// 지정한 유저의 암호를 변경한다.
        /// </summary>
        /// <param name="user">변경하려는 사용자.</param>
        /// <param name="newPassword">바꾸려는 새 암호.</param>
        internal static void ChangePassword(User user, string newPassword)
        {
            // 새 암호 만들기.
            var newUser = CreateUser(null, newPassword);
            user.Password = newUser.Password;
            user.Salt = newUser.Salt;
        }

        /// <summary>
        /// 중복되지 않는 새 사용자 이름을 만든다.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        internal string GetNewUserName(string baseName, int startIndex = 1)
        {
            while (startIndex < int.MaxValue)
            {
                var newName = $"{baseName}_{startIndex}";
                if (GetUser(newName) == null)
                {
                    return newName;
                }
                startIndex++;
            }

            return baseName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss");
        }
    }
}
