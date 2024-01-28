using Microsoft.AspNetCore.Mvc;
using System.Security;
using System.Security.Permissions;
using TopLearn.Core.Services;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
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

        [Route("AdminPanel/Roles/CreateRole")]
        public IActionResult CreateRole()
        {
            ViewBag.Permission = _permission.GetAllPermission();
            return View();
        }
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

        [Route("AdminPanel/Roles/EditRole")]
        public IActionResult EditRole(int RoleId)
        {
            ViewBag.Permission = _permission.GetAllPermission();
            ViewBag.SelectedPermission = _permission.PermissionsRole(RoleId);
            return View(_permission.GetByRoleId(RoleId));
        }
        [Route("AdminPanel/Roles/EditRole")]
        [HttpPost]
        public IActionResult EditRole(Role role)
        {
            if (!ModelState.IsValid && role.UserRoles != null)
            {
                return View(role);
            }
            role.IsDeleted = false;
            _permission.UpdateRole(role);
            return Redirect("/AdminPanel/Roles");
        }

        #endregion

        #region DeleteRole

        [Route("AdminPanel/Roles/DeleteRole")]
        public IActionResult DeleteRole(int RoleId)
        {
            return View(_permission.GetByRoleId(RoleId));
        }
        [HttpPost]
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
