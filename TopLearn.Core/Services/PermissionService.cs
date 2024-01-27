using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services
{
    public class PermissionService : IPermissionService
    {
        TopLearnContext _context;
        public PermissionService(TopLearnContext topLearnContext)
        {
            _context = topLearnContext;
        }

        public List<Role> Roles()
        {
            return _context.Roles.ToList();
        }

        public void UpdateRolesToUsers(List<int> RoleIds, int UserId)
        {
            foreach (int roleId in RoleIds)
            {
                _context.UserRoles.Update(new UserRole()
                {
                    RoleId = roleId,
                    UserId = UserId
                });
            }
            _context.SaveChanges();
        }

        void IPermissionService.AddRolesToUsers(List<int> RoleIds, int UserId)
        {
            foreach (int roleId in RoleIds)
            {
                _context.UserRoles.Add(new UserRole()
                {
                    RoleId = roleId,
                    UserId = UserId
                });
            }
            _context.SaveChanges();
        }
    }
}
