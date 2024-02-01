using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Core.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using TopLearn.DataLayer.Entities.Course;

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
            return View(_service.GetCoursesForAdmin());
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
        [PermissionChecker(11)]
        [HttpPost]
        [Route("AdminPanel/CreateCourse")]
        public IActionResult CreateCourse(Course course, IFormFile ImageCourse, IFormFile DemoImageCourse)
        {
            if (!ModelState.IsValid && ImageCourse == null && DemoImageCourse == null)
                return View(course);
            _service.AddCourse(course, ImageCourse, DemoImageCourse);
            return Redirect("Index");
        }
        [PermissionChecker(12)]
        [Route("AdminPanel/EditCourse")]
        public IActionResult EditCourse(int courseId)
        {
            var CourseForEdit = _service.GetCourseById(courseId);
            var Groups = _service.GetGroupForManageCourse();
            ViewBag.GroupCourse = new SelectList(Groups, "Value", "Text", CourseForEdit.GroupId);

            var SubGroups = _service.GetSubGroupForManageCourse(int.Parse(Groups.FirstOrDefault().Value));
            ViewBag.SubGroupCourse = new SelectList(SubGroups, "Value", "Text", CourseForEdit.SubGroup ?? 0);

            var Teachers = _service.GetTeacher();
            ViewBag.Teachers = new SelectList(Teachers, "Value", "Text", CourseForEdit.TeacherId);

            var Status = _service.GetStatus();
            ViewBag.Statues = new SelectList(Status, "Value", "Text", CourseForEdit.LevelId);

            var Levels = _service.GetLevel();
            ViewBag.Levels = new SelectList(Levels, "Value", "Text", CourseForEdit.StatusId);

            return View(CourseForEdit);
        }
        [PermissionChecker(12)]
        [HttpPost]
        [Route("AdminPanel/EditCourse")]
        public IActionResult EditCourse(Course CourseForEdit, IFormFile ImageCourse, IFormFile DemoImageCourse)
        {
            _service.UpdateCourse(CourseForEdit, ImageCourse, DemoImageCourse);
            return Redirect("Index");

        }
        [PermissionChecker(10)]
        [HttpGet]
        [Route("AdminPanel/IndexEpisode")]
        public IActionResult IndexEpisode(int courseId)
        {
            ViewBag.CourseId = courseId;
            return View(_service.GetEpisodes(courseId));
        }
        [PermissionChecker(11)]
        [HttpGet]
        [Route("AdminPanel/CreateEpisode")]
        public IActionResult CreateEpisode(int courseId)
        {
            return View();
        }
        [PermissionChecker(11)]
        [HttpPost]
        [Route("AdminPanel/CreateEpisode")]
        public IActionResult CreateEpisode(CourseEpisode courseEpisode, IFormFile fileEpisode)
        {
            if (!ModelState.IsValid && fileEpisode == null)
                return View(courseEpisode);
            if (_service.ChechExistFile(fileEpisode.FileName))
            {
                ViewBag.IsExistFile = true;
                return View(courseEpisode);
            }
            _service.AddEpisode(courseEpisode, fileEpisode);
            return Redirect("IndexEpisode?courseId=" + courseEpisode.CourseId);
        }
        [PermissionChecker(12)]
        [HttpGet]
        [Route("AdminPanel/EditEpisode")]
        public IActionResult EditEpisode(int episodeId)
        {
            return View(_service.GetEpisodesById(episodeId));
        }
        [PermissionChecker(12)]
        [HttpPost]
        [Route("AdminPanel/EditEpisode")]
        public IActionResult EditEpisode(CourseEpisode courseEpisode, IFormFile fileEpisode)
        {
            if (!ModelState.IsValid && courseEpisode == null)
                return View(courseEpisode);
            if (_service.ChechExistFile(fileEpisode.FileName))
            {
                ViewBag.IsExistFile = true;
                return View(courseEpisode);
            }
            _service.UpdateEpisode(courseEpisode, fileEpisode);
            return Redirect("IndexEpisode?courseId=" + courseEpisode.CourseId);
        }
        [PermissionChecker(12)]
        [HttpGet]
        [Route("AdminPanel/DeleteEpisode")]
        public IActionResult DeleteEpisode(int episodeId)
        {
            return View(_service.GetEpisodesById(episodeId));
        }
        [PermissionChecker(12)]
        [HttpPost]
        [Route("AdminPanel/DeleteEpisode")]
        public IActionResult DeleteEpisode(CourseEpisode courseEpisode)
        {
            if (!ModelState.IsValid && courseEpisode == null)
                return View(courseEpisode);
            _service.DeleteEpisode(courseEpisode);
            return Redirect("IndexEpisode?courseId=" + courseEpisode.CourseId);
        }
    }
}
