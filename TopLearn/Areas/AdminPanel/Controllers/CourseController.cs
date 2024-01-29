using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Core.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TopLearn.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [PermissionChecker(10)]
    public class CourseController : Controller
    {
        ICourseService _service;
        public CourseController(ICourseService courseService)
        {
            _service = courseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [PermissionChecker(11)]
        [Route("AdminPanel/CreateCourse")]
        public IActionResult CreateCourse()
        {
            var Groups = _service.GetGroupForManageCourse();
            ViewBag.GroupCourse = new SelectList(Groups, "Value", "Text");

            var SubGroups = _service.GetSubGroupForManageCourse(int.Parse(Groups.FirstOrDefault().Value));
            ViewBag.SubGroupCourse = new SelectList(SubGroups, "Value", "Text");

            var Teachers = _service.GetTeacher();
            ViewBag.Teachers = new SelectList(Teachers, "Value", "Text");

            var Status = _service.GetStatus();
            ViewBag.Statues = new SelectList(Status, "Value", "Text");

            var Levels = _service.GetLevel();
            ViewBag.Levels = new SelectList(Levels, "Value", "Text");

            return View();
        }

    }
}
