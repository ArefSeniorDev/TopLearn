using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services
{
    public interface IPermissionService
    {
        List<Role> Roles();
        void AddRolesToUsers(List<int> RoleIds, int UserId);
        void UpdateRolesToUsers(List<int> RoleIds, int UserId);
    }
}
