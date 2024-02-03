using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Migrations;

namespace TopLearn.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class MyOrderController : Controller
    {
        IOrderService _orderService;
        IUserService _userService;
        public MyOrderController(IOrderService order, IUserService userService)
        {
            _orderService = order;
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ShowOrder(int Id, bool finaly = false)
        {
            ViewBag.WalletBallance = _userService.BalanceUserWallet(User.Identity.Name);
            var order = _orderService.GetOrderForUserPanel(User.Identity.Name, Id);
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.finaly = finaly;
            ViewBag.IsArchive = null;
            if (finaly)
            {
                ViewBag.IsArchive = true;
            }
            return View(order);
        }
        public IActionResult FinallyOrder(int Id)
        {
            if (_orderService.FinallyOrder(User.Identity.Name, Id))
            {
                return Redirect("/UserPanel/ShowOrder/" + Id + "?finaly=true");
            }
            return BadRequest();
        }
    }
}
