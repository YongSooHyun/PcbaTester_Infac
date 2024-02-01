using System;
using System.Collections.Generic;
using System.Text;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 사용자의 권한을 나타낸다.
    /// </summary>
    public class UserPermission
    {
        ///// <summary>
        ///// Initilaize 기능을 사용할 수 있는지 여부.
        ///// </summary>
        //public bool CanInitialize { get; set; } = true;

        ///// <summary>
        ///// Fixture 교환 기능을 사용할 수 있는지 여부.
        ///// </summary>
        //public bool CanChangeFixture { get; set; } = true;

        /// <summary>
        /// 제품 정보를 편집할 수 있는지 여부.
        /// </summary>
        public bool CanEditProdcut { get; set; } = true;

        /// <summary>
        /// COM 포트 설정을 편집할 수 있는지 여부.
        /// </summary>
        public bool CanEditComPorts { get; set; } = true;

        /// <summary>
        /// 사용자들을 관리할 수 있는지 여부.
        /// </summary>
        public bool CanManageUsers { get; set; } = true;

        /// <summary>
        /// Project 편집 -> Master Good/Fail 편집 여부.
        /// </summary>
        public bool CanEditMasterProject { get; set; } = true;

        /// <summary>
        /// 인스턴스를 생성하고 주어진 역할에 따른 권한 설정을 한다.
        /// </summary>
        /// <param name="role">사용자 역할.</param>
        public static UserPermission CreateByRole(UserRole role)
        {
            var permission = new UserPermission();
            switch (role)
            {
                case UserRole.작업자:
                    permission.CanEditProdcut = false;
                    permission.CanEditComPorts = false;
                    permission.CanManageUsers = false;
                    permission.CanEditMasterProject = false;
                    break;
                case UserRole.기술자:
                    permission.CanManageUsers = false;
                    permission.CanEditMasterProject = false;
                    break;
                case UserRole.관리자:
                default:
                    break;
            }
            return permission;
        }
    }
}
