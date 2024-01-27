using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.DTOs.UserViewModel;
using TopLearn.Core.Services;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Areas.UserPanel.Controllers
{
    [Authorize]
    [Area("UserPanel")]
    public class WalletController : Controller
    {
        IUserInterface _service;
        public WalletController(IUserInterface user)
        {
            _service = user;
        }
        [Route("UserPanel/Wallet")]
        public IActionResult Index()
        {
            ViewBag.ListWallet = _service.GetUserWallet(User.Identity.Name);
            return View();
        }
        [HttpPost]
        [Route("UserPanel/Wallet")]
        public IActionResult Index(ChargeWalletViewModel charge)
        {
            string UserName = User.Identity.Name;
            if (!ModelState.IsValid)
            {
                ViewBag.ListWallet = _service.GetUserWallet(UserName);
                return View(charge);
            }

            _service.AddMoney(UserName, charge.Amount, "شارژ حساب", true);
            ViewBag.IsSuccess = true;
            ViewBag.ListWallet = _service.GetUserWallet(UserName);
            return View();
        }
    }
}
