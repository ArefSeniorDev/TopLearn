using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.Permissions;
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

        public List<Role> GetRoles()
        {
            return _context.Roles.ToList();
        }

        public void AddRolesToUsers(List<int> RoleIds, int UserId)
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
        public void UpdateRolesToUsers(List<int> RoleIds, int UserId)
        {
            //Delete All UserRoles
            _context.UserRoles.Where(x => x.UserId == UserId).ToList().ForEach(x => _context.UserRoles.Remove(x));
            //Add All Roles
            AddRolesToUsers(RoleIds, UserId);
        }

        public int AddRole(Role role)
        {
            _context.Add(role);
            _context.SaveChanges();
            return role.RoleId;
        }

        public Role GetByRoleId(int roleId)
        {
            return _context.Roles.Find(roleId);
        }

        public void UpdateRole(Role role)
        {
            _context.Update(role);
            _context.SaveChanges();
        }

        public void DeleteRole(int roleId)
        {
            var Role = GetByRoleId(roleId);
            Role.IsDeleted = true;
            UpdateRole(Role);
        }

        public List<Permission> GetAllPermission()
        {
            return _context.Permission.ToList();
        }

        public void AddPermissionToRole(int RoleId, List<int> permission)
        {
            foreach (var p in permission)
            {

                _context.RolePermission.Add(new RolePermission()
                {
                    PermissionId = p,
                    RoleId = RoleId,
                });
            }
            _context.SaveChanges();

        }

        public List<int> PermissionsRole(int RoleId)
        {
            return _context.RolePermission.Where(x => x.RoleId == RoleId).Select(x => x.PermissionId).ToList();
        }

        public void UpdatePermissionRole(int RoleId, List<int> Permission)
        {
            _context.RolePermission.Where(x => x.RoleId == RoleId).ToList().ForEach(x => _context.RolePermission.Remove(x));

            AddPermissionToRole(RoleId, Permission);
        }

        public bool CheckPermission(int permissionId, string userName)
        {
            int UserId = _context.Users.SingleOrDefault(x => x.UserName == userName).UserId;

            List<int> UserRoles = _context.UserRoles.Where(x => x.UserId == UserId).Select(x => x.RoleId).ToList();

            if (!UserRoles.Any())
                return false;

            List<int> RolePermission = _context.RolePermission.Where(x => x.PermissionId == permissionId).Select(x => x.RoleId).ToList();

            return RolePermission.Any(x => UserRoles.Contains(x));
        }
    }
}
