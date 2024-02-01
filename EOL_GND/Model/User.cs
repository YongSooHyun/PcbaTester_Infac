using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    public enum UserRole
    {
        /// <summary>
        /// 작업자. 시퀀스 및 설정을 수정할 수 없음.
        /// </summary>
        Operator = 7,

        /// <summary>
        /// 기술자. 사용자 관리를 제외한 모든 설정 가능.
        /// </summary>
        Engineer = 23,

        /// <summary>
        /// 관리자. 모든 기능을 사용할 수 있음.
        /// </summary>
        Administrator = 47,
    }

    /// <summary>
    /// 로그인 할 수 있는 유저 정보를 관리한다.
    /// </summary>
    public class User
    {
        /// <summary>
        /// 사용자 ID.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 사용자 Password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Password 설정에 이용된다.
        /// </summary>
        [XmlIgnore]
        public string PlainPassword
        {
            get => _plainPassword;
            set
            {
                UserManager.ChangePassword(this, value);
                _plainPassword = value;
            }
        }
        private string _plainPassword;

        /// <summary>
        /// Password 보안에 이용되는 Salt.
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// 사용자 등급.
        /// </summary>
        public UserRole Role { get; set; } = UserRole.Operator;

        /// <summary>
        /// 메모.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 사용자의 권한 정보를 리턴한다.
        /// </summary>
        /// <returns></returns>
        internal UserPermission GetPermission()
        {
            return UserPermission.CreateByRole(Role);
        }
    }
}
