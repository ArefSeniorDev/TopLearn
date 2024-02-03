using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Controllers
{
    public class ShowCourseController : Controller
    {
        ICourseService _service;
        IOrderService _orderService;
        public ShowCourseController(ICourseService courseService, IOrderService orderService)
        {
            _service = courseService;
            _orderService = orderService;

        }
        public IActionResult Index(int pageId = 1, string filter = "", string getType = "all",
            string orderByType = "date", int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null)
        {
            ViewBag.selectedGroups = selectedGroups;
            ViewBag.Groups = _service.GetAllGroup();
            ViewBag.pageId = pageId;
            return View(_service.GetCourse(pageId, filter, getType, orderByType, startPrice, endPrice, selectedGroups, 9));

        }
        [Route("ShowCourse/{Id}")]
        public IActionResult ShowCourse(int Id)
        {
            var Sincourse = _service.GetCourseForShow(Id);
            if (Sincourse == null)
            {
                return NotFound();
            }
            return View(Sincourse);
        }
        [Route("BuyCourse/{Id}")]
        [Authorize]
        public IActionResult BuyCourse(int Id)
        {
            int Order = _orderService.AddOrder(User.Identity.Name,Id);
            return Redirect("/UserPanel/MyOrder/ShowOrder/" + Order);

        }
    }
}
