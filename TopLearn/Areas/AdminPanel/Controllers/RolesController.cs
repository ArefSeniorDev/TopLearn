using Microsoft.AspNetCore.Mvc;
using System.Security;
using System.Security.Permissions;
using TopLearn.Core.Security;
using TopLearn.Core.Services;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [PermissionChecker(6)]
    public class RolesController : Controller
    {
        IPermissionService _permission;
        public RolesController(IPermissionService permission)
        {
            _permission = permission;
        }
        public IActionResult Index()
        {
            return View(_permission.GetRoles());
        }

        #region Create Role
        [PermissionChecker(7)]

        [Route("AdminPanel/Roles/CreateRole")]
        public IActionResult CreateRole()
        {
            ViewBag.Permission = _permission.GetAllPermission();
            return View();
        }

        [PermissionChecker(7)]
        [HttpPost]
        [Route("AdminPanel/Roles/CreateRole")]
        public IActionResult CreateRole(Role role, List<int> SelectedPermission)
        {
            if (!ModelState.IsValid && role.UserRoles != null)
            {
                return View(role);
            }
            role.IsDeleted = false;
            int RoleId = _permission.AddRole(role);
            _permission.AddPermissionToRole(RoleId, SelectedPermission);
            return Redirect("/Adminpanel/Roles");
        }

        #endregion

        #region Edit Role
        [PermissionChecker(8)]
        [Route("AdminPanel/Roles/EditRole")]
        public IActionResult EditRole(int RoleId)
        {
            ViewBag.Permission = _permission.GetAllPermission();
            ViewBag.SelectedPermission = _permission.PermissionsRole(RoleId);
            return View(_permission.GetByRoleId(RoleId));
        }

        [PermissionChecker(8)]
        [Route("AdminPanel/Roles/EditRole")]
        [HttpPost]
        public IActionResult EditRole(Role role, List<int> SelectedPermission)
        {
            if (!ModelState.IsValid && role.UserRoles != null)
            {
                return View(role);
            }
            role.IsDeleted = false;
            _permission.UpdateRole(role);
            _permission.UpdatePermissionRole(role.RoleId,SelectedPermission);

            return Redirect("/AdminPanel/Roles");
        }

        #endregion

        #region DeleteRole
        [PermissionChecker(9)]
        [Route("AdminPanel/Roles/DeleteRole")]
        public IActionResult DeleteRole(int RoleId)
        {
            return View(_permission.GetByRoleId(RoleId));
        }

        [HttpPost]
        [PermissionChecker(9)]
        [Route("AdminPanel/Roles/DeleteRole")]
        public IActionResult DeleteRole(Role role)
        {
            if (!ModelState.IsValid && role.UserRoles != null)
            {
                return View(role);
            }
            role.IsDeleted = false;
            _permission.DeleteRole(role.RoleId);
            return Redirect("/Adminpanel/Roles");
        }


        #endregion
    }
}
