using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IntelligentPcbaTester
{
    public enum UserRole
    {
        /// <summary>
        /// 작업자.
        /// </summary>
        작업자 = 1,

        /// <summary>
        /// 기술자.
        /// </summary>
        기술자 = 2,

        /// <summary>
        /// 관리자.
        /// </summary>
        관리자 = 3,
    }

    /// <summary>
    /// 로그인 할 수 있는 유저 정보를 관리한다.
    /// </summary>
    public class User
    {
        /// <summary>
        /// 사용자 ID.
        /// </summary>
        [DisplayName("사용자 이름")]
        public string UserName { get; set; }

        /// <summary>
        /// 사용자 Password.
        /// </summary>
        [DisplayName("Password")]
        public string Password { get; set; }

        /// <summary>
        /// 사용자 등급.
        /// </summary>
        [DisplayName("등급")]
        public UserRole Role { get; set; } = UserRole.관리자;

        /// <summary>
        /// 메모.
        /// </summary>
        [DisplayName("비고")]
        public string Note { get; set; }

        internal UserPermission GetPermission()
        {
            var permission = UserPermission.CreateByRole(Role);
            return permission;
        }
    }
}
