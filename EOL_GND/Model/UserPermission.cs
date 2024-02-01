using System;
using System.Collections.Generic;
using System.Text;

namespace EOL_GND.Model
{
    /// <summary>
    /// 사용자의 권한을 나타낸다.
    /// </summary>
    public class UserPermission
    {
        /// <summary>
        /// 시퀀스를 편집할 수 있는지 여부.
        /// </summary>
        public bool CanEditSequence { get; set; }

        /// <summary>
        /// 폰트, 윈도우 상태 복원, 로그 등 일반 설정을 편집할 수 있는지 여부.
        /// </summary>
        public bool CanEditGeneralSettings { get; set; }

        /// <summary>
        /// 전문가용 설정을 편집할 수 있는지 여부.
        /// </summary>
        public bool CanEditExpertSettings { get; set; }

        /// <summary>
        /// CAN Message 관련 설정을 편집할 수 있는지 여부.
        /// </summary>
        public bool CanEditCanMessageSettings { get; set; }

        /// <summary>
        /// 디바이스 설정을 편집할 수 있는지 여부.
        /// </summary>
        public bool CanEditDeviceSettings { get; set; }

        /// <summary>
        /// 사용자들을 관리할 수 있는지 여부.
        /// </summary>
        public bool CanManageUsers { get; set; }

        /// <summary>
        /// 인스턴스를 생성하고 주어진 역할에 따른 권한 설정을 한다.
        /// </summary>
        /// <param name="role">사용자 역할.</param>
        public static UserPermission CreateByRole(UserRole role)
        {
            var permission = new UserPermission();
            switch (role)
            {
                case UserRole.Engineer:
                    permission.CanEditSequence = true;
                    permission.CanEditGeneralSettings = true;
                    permission.CanEditExpertSettings = true;
                    permission.CanEditCanMessageSettings = true;
                    permission.CanEditDeviceSettings = true;
                    permission.CanManageUsers = false;
                    break;
                case UserRole.Administrator:
                    permission.CanEditSequence = true;
                    permission.CanEditGeneralSettings = true;
                    permission.CanEditExpertSettings = true;
                    permission.CanEditCanMessageSettings = true;
                    permission.CanEditDeviceSettings = true;
                    permission.CanManageUsers = true;
                    break;
                case UserRole.Operator:
                default:
                    permission.CanEditSequence = false;
                    permission.CanEditGeneralSettings = false;
                    permission.CanEditExpertSettings = false;
                    permission.CanEditCanMessageSettings = false;
                    permission.CanEditDeviceSettings = false;
                    permission.CanManageUsers = false;
                    break;
            }
            return permission;
        }
    }
}
