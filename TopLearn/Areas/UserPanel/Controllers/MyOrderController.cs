using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using TopLearn.Core.DTOs.Enum;
using TopLearn.Core.Services.Interfaces;


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
            return View(_orderService.GetUserOrders(User.Identity.Name));
        }
        public IActionResult ShowOrder(int Id, bool finaly = false, string type = "")
        {
            ViewBag.WalletBallance = _userService.BalanceUserWallet(User.Identity.Name);
            var order = _orderService.GetOrderForUserPanel(User.Identity.Name, Id);
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.DiscountType = type;
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
                return Redirect("/UserPanel/MyOrder/ShowOrder/" + Id + "?finaly=true");
            }
            return BadRequest();
        }
        public IActionResult UseDiscount(int orderId, string code)
        {
            DiscountUseType Type = _orderService.UseDiscount(orderId, code);
            return Redirect("/UserPanel/MyOrder/ShowOrder/" + orderId + "?type=" + Type.ToString());
        }
    }
}
