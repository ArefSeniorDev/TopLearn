using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.DataLayer.Entities.Permissions;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services
{
    public interface IPermissionService
    {
        #region Role

        List<Role> GetRoles();
        void AddRolesToUsers(List<int> RoleIds, int UserId);
        void UpdateRolesToUsers(List<int> RoleIds, int UserId);
        int AddRole(Role role);
        void UpdateRole(Role role);
        void DeleteRole(int roleId);
        Role GetByRoleId(int roleId);
        #endregion


        #region Permission

        List<Permission> GetAllPermission();
        void AddPermissionToRole(int RoleId, List<int> permission);
        List<int> PermissionsRole(int RoleId);

        #endregion
    }
}
