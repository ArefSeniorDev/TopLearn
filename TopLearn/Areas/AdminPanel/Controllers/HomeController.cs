using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs.RolesViewModel;
using TopLearn.Core.DTOs.UserViewModel;
using TopLearn.Core.Services;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

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
            ViewBag.Roles = _permission.GetRoles();
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
            if (_service.IsEmailExist(TextFixer.TextFixed(createUserViewModel.Email)) || _service.IsExistUserName(createUserViewModel.UserName))
            {
                ModelState.AddModelError("Email", "ایمیل  یا  نام  کاربری  ثبت  شده  است");
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
            ViewBag.Roles = _permission.GetRoles();
            return View(_service.GetByUserIdForEditAdmin(UserId));
        }
        [HttpPost]
        [Route("AdminPanel/EditUser")]
        public IActionResult EditUser(List<int> SelectedRoles, EditUserViewModel editUserViewModel)
        {

            if (!ModelState.IsValid && editUserViewModel.UserAvatar != null && editUserViewModel.UserRoles != null)
            {
                return View(editUserViewModel);
            }
            int UserId = _service.UpdateUserFromAdmin(editUserViewModel);

            _permission.UpdateRolesToUsers(SelectedRoles, UserId);

            return Redirect("/AdminPanel/Users");
        }

        #endregion


        #region Deleted User

        [Route("AdminPanel/DeletedUsers")]
        public IActionResult DeletedUsers(int PageId = 1, string filterUserName = "", string filterEmail = "")
        {
            var user = _service.GetDeletedUsers(PageId, filterUserName, filterEmail);
            return View(user);
        }
        #endregion

        #region Delete User

        [Route("AdminPanel/DeleteUser")]
        public IActionResult DeleteUser(int UserId)
        {
            InformationUserViewModel information = new InformationUserViewModel();
            information = _service.GetUserInformation(UserId);
            ViewBag.UserId = UserId;
            return View(information);
        }
        [Route("AdminPanel/DeleteUser")]
        [HttpPost]
        public IActionResult DeleteUser(string UserId)
        {
            _service.DeletingUser(int.Parse(UserId));
            return Redirect("/AdminPanel/Users");
        }
        #endregion
    }
}
