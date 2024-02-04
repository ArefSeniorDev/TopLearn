using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class DiscountController : Controller
    {
        IOrderService _service;
        public DiscountController(IOrderService orderService)
        {
            _service = orderService;
        }
        public IActionResult Index()
        {
            return View(_service.GetAllDiscounts());
        }
        public IActionResult CreateDiscount()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateDiscount(Discount discount, string stDate = "", string edDate = "")
        {
            if (!string.IsNullOrEmpty(stDate))
            {
                string[] std = stDate.Split('/');
                discount.StartTime = new DateTime(int.Parse(std[0]),
                    int.Parse(std[1]),
                    int.Parse(std[2]),
                    new PersianCalendar()
                    );
            }

            if (!string.IsNullOrEmpty(edDate))
            {
                string[] edd = edDate.Split('/');
                discount.EndTime = new DateTime(int.Parse(edd[0]),
                    int.Parse(edd[1]),
                    int.Parse(edd[2]),
                    new PersianCalendar()
                );
            }
            if (_service.IsExistCode(discount.DiscountCode))
            {
                ModelState.AddModelError("DiscountCode","نمیتواند تکراری باشد");
                return View(discount);
            }
            if (!ModelState.IsValid && discount == null)
            {
                return View(discount);
            }
            _service.AddDiscount(discount);
            return Redirect("/AdminPanel/Discount");
        }
        [Route("/AdminPanel/Discount/EditDiscount/{Id}")]
        public IActionResult EditDiscount(int Id)
        {
            return View(_service.GetByDiscountId(Id));
        }
        [HttpPost]
        [Route("/AdminPanel/Discount/EditDiscount/{Id}")]
        public IActionResult EditDiscount(Discount discount, string stDate = "", string edDate = "")
        {
            if (!string.IsNullOrEmpty(stDate))
            {
                string[] std = stDate.Split('/');
                discount.StartTime = new DateTime(int.Parse(std[0]),
                    int.Parse(std[1]),
                    int.Parse(std[2]),
                    new PersianCalendar()
                    );
            }

            if (!string.IsNullOrEmpty(edDate))
            {
                string[] edd = edDate.Split('/');
                discount.EndTime = new DateTime(int.Parse(edd[0]),
                    int.Parse(edd[1]),
                    int.Parse(edd[2]),
                    new PersianCalendar()
                );
            }
            if (!ModelState.IsValid && discount == null)
            {
                return View(discount);
            }
            _service.UpdateDiscount(discount);
            return Redirect("/AdminPanel/Discount");
        }
        [Route("/AdminPanel/Discount/DeleteDiscount/{Id}")]
        public IActionResult DeleteDiscount(int Id)
        {
            return View(_service.GetByDiscountId(Id));
        }
        [Route("/AdminPanel/Discount/DeleteDiscount/{Id}")]
        [HttpPost]
        public IActionResult DeleteDiscount(Discount discount)
        {
            _service.DeleteDiscount(discount);
            return Redirect("/AdminPanel/Discount");
        }
    }
}
