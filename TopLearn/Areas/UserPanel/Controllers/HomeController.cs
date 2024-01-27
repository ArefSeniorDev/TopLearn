using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Core.DTOs.UserViewModel;

namespace TopLearn.Areas.UserPanel.Controllers
{
    [Authorize]
    [Area("UserPanel")]
    public class HomeController : Controller
    {
        IUserInterface _service;
        public HomeController(IUserInterface user)
        {
            _service = user;
        }
        public IActionResult Index()
        {
            return View(_service.GetUserInformation(User.Identity.Name));
        }


        #region EditProfile
        [Route("UserPanel/EditProfile")]
        public IActionResult EditProfile()
        {
            return View(_service.GetEditProfile(User.Identity.Name));
        }
        [HttpPost]
        [Route("UserPanel/EditProfile")]
        public IActionResult EditProfile(EditProfileViewModel editProfile)
        {
            if (!ModelState.IsValid)
                return View(editProfile);

            _service.EditProfile(User.Identity.Name, editProfile);

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            return Redirect("/Login?EditProfile=true");
        }
        #endregion

        #region ChangePass
        [Route("UserPanel/ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [Route("UserPanel/ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordViewModel change)
        {
            if (!ModelState.IsValid)
                return View(change);
            string UserName = User.Identity.Name;
            if (!_service.CheckTheOldPass(UserName,change.OldPassword))
            {
                ModelState.AddModelError("OldPassword", "کلمه عبور فعلی همخوانی ندارد");
                return View(change);
            }
            _service.EditPassWord(UserName, change.Password);
            ViewBag.IsSuccess = true;
            return View();
        }
        #endregion
    }
}
