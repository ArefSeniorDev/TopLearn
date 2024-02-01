using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using NuGet.Protocol.Plugins;
using System.Security.Claims;
using TopLearn.Core.Convertors;
using TopLearn.Core.Genrator;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.User;
using TopLearn.Core.DTOs.UserViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TopLearn.Controllers
{
    public class AccountController : Controller
    {
        IUserInterface _service;
        ICourseService _courseservice;
        IViewRenderService _viewrender;
        public AccountController(IUserInterface service, IViewRenderService viewRender, ICourseService courseservice)
        {
            _service = service;
            _viewrender = viewRender;
            _courseservice = courseservice;
        }


        #region Register

        [Route("Register"), HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [Route("Register"), HttpPost]
        public IActionResult Register(AccountViewModel register)
        {



            if (!ModelState.IsValid)
            {
                return View(register);
            }
            if (_service.IsEmailExist(TextFixer.TextFixed(register.Email)) || _service.IsExistUserName(register.UserName))
            {
                ModelState.AddModelError("Email", "ایمیل  یا  نام  کاربری  ثبت  شده  است");
                return View(register);
            }

            User user = new User()
            {
                UserName = register.UserName,
                Password = PasswordHelper.EncodePasswordMd5(register.Password),
                Email = register.Email,
                ActiveCode = GetUserActiveCode.GetActiveCode(),
                IsActive = false,
                RegisterDate = DateTime.Now,
                UserAvatar = "Defult.jpg",
            };
            _service.AddUser(user);

            #region Active Account 
            string body = _viewrender.RenderToStringAsync("_ActiveEmail", user);
            SendEmail.Send(user.Email, "فعالسازی", body);

            #endregion

            return View("SuccessRegister", user);
        }
        #endregion


        #region Login
        [Route("Login")]
        public IActionResult Login(bool EditProfile = false)
        {
            ViewBag.EditProfile = EditProfile;
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginViewModel loginViewModel)
        {

            //if (!ModelState.IsValid)
            //{
            //    return View(loginViewModel);
            //}
            var user = _service.LoginUser(loginViewModel);
            if (user != null)
            {
                if (user.IsActive)
                {
                    //TODO
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
               // new Claim("CodeMeli", user.Email),

            };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = loginViewModel.RememberMe
                    };

                    HttpContext.SignInAsync(principal, properties);
                    ViewBag.IsSuccess = user.Email;
                }
                else
                {
                    ModelState.AddModelError("Email", "حساب کاربری شما فعال نمیباشد!");
                }
            }
            else
            {
                ModelState.AddModelError("Email", "کاربری با این مشخصات یافت نشد!");
                return View(loginViewModel);
            }


            return View();
        }
        #endregion

        #region ActiveAccount
        public IActionResult ActiveAccount(string id)
        {
            ViewBag.IsActive = _service.ActiveAccount(id);
            return View();
        }
        #endregion

        #region Logout
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");

        }

        #endregion
        #region ForgetPass

        [Route("ResetPassword")]

        public IActionResult ResetPassword(string Id)
        {
            return View(new ResetPasswordViewModel()
            {
                ActiveCode = Id
            });
        }
        [HttpPost]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(ResetPasswordViewModel reset)
        {
            if (!ModelState.IsValid)
                return View(reset);
            var user = _service.GetByActiveCode(reset.ActiveCode);
            string HasNewPass = PasswordHelper.EncodePasswordMd5(reset.Password);
            user.Password = HasNewPass;
            _service.UpdateUser(user);
            return Redirect("/Login");
        }
        #endregion

        #region ResetPass

        [Route("ForgotPassword")]

        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword(ForgotPasswordViewModel forgot)
        {
            if (!ModelState.IsValid)
                return View(forgot);

            string Fixed = TextFixer.TextFixed(forgot.Email);
            var User = _service.GetByEmail(Fixed);
            if (User == null)
            {
                ModelState.AddModelError("Email", "کاربری با این مشخصات یافت نشد");
                return View(forgot);
            }
            string viewbody = _viewrender.RenderToStringAsync("_ForgotPassword", User);
            SendEmail.Send(User.Email, "بازیابی حساب کاربری", viewbody);
            ViewBag.IsSuccess = true;
            return View();
        }

        #endregion


        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult GetSubGroup(int Id)
        {
            List<SelectListItem> list = new List<SelectListItem>()
            {
                new SelectListItem(){Text="انتخاب کنید",Value=""}
            };
            list.AddRange(_courseservice.GetSubGroupForManageCourse(Id));
            return Json(new SelectList(list, "Value", "Text"));
        }

        [HttpPost]
        [Route("file-upload")]
        public IActionResult UploadImage(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            if (upload.Length <= 0) return null;

            var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();



            var path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/MyImages",
                fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                upload.CopyTo(stream);

            }



            var url = $"{"/MyImages/"}{fileName}";


            return Json(new { uploaded = true, url });
        }
    }
}
