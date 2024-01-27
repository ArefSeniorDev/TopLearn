using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.DTOs.UserViewModel;
using TopLearn.Core.Services;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class HomeController : Controller
    {
        IUserInterface _service;
        IPermissionService _permission;
        public HomeController(IUserInterface userInterface, IPermissionService permissionService)
        {
            _service = userInterface;
            _permission = permissionService;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region UsersPreview
        [Route("AdminPanel/Users")]
        public IActionResult Users(int PageId = 1, string filterUserName = "", string filterEmail = "")
        {
            var user = _service.GetUsers(PageId, filterUserName, filterEmail);
            return View(user);
        }
        #endregion


        #region CreateUser
        [Route("AdminPanel/CreateUser")]
        public IActionResult CreateUser()
        {
            ViewBag.Roles = _permission.Roles();
            return View();
        }
        [HttpPost]
        [Route("AdminPanel/CreateUser")]
        public IActionResult CreateUser(List<int> SelectedRoles, CreateUserViewModel createUserViewModel)
        {

            if (!ModelState.IsValid && createUserViewModel.UserAvatar != null)
            {
                return View(createUserViewModel);
            }
            int UserId = _service.AddUserFromAdmin(createUserViewModel);

            _permission.AddRolesToUsers(SelectedRoles, UserId);

            ViewBag.IsSuccess = true;

            return Redirect("/AdminPanel/Users");
        }

        #endregion
        #region EditUser
        [Route("AdminPanel/EditUser")]
        public IActionResult EditUser(int UserId)
        {
            ViewBag.Roles = _permission.Roles();
            return View(_service.GetByUserIdForEditAdmin(UserId));
        }
        [HttpPost]
        [Route("AdminPanel/EditUser")]
        public IActionResult EditUser(List<int> SelectedRoles, CreateUserViewModel createUserViewModel)
        {

            if (!ModelState.IsValid && createUserViewModel.UserAvatar != null)
            {
                return View(createUserViewModel);
            }
            int UserId = _service.UpdateUserFromAdmin(createUserViewModel);

            _permission.UpdateRolesToUsers(SelectedRoles, UserId);

            return Redirect("/AdminPanel/Users");
        }

        #endregion


    }
}
