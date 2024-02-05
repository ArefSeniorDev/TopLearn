using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Controllers
{
    public class ShowCourseController : Controller
    {
        ICourseService _service;
        IOrderService _orderService;
        IUserService _userService;
        public ShowCourseController(ICourseService courseService, IOrderService orderService, IUserService userService)
        {
            _service = courseService;
            _orderService = orderService;
            _userService = userService;
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
            int Order = _orderService.AddOrder(User.Identity.Name, Id);
            return Redirect("/UserPanel/MyOrder/ShowOrder/" + Order);

        }
        [Route("DownloadFile/{episodeId}")]
        public IActionResult DownloadFile(int episodeId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Login");
            }
            if (User.Identity.IsAuthenticated)
            {
                var episode = _service.GetEpisodesById(episodeId);
                string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course_ep", episode.EpisodeFileName);
                string FileName = episode.EpisodeFileName;
                if (episode.IsFree)
                {
                    byte[] file = System.IO.File.ReadAllBytes(FilePath);
                    return File(file, "application/force-download", FileName);
                }
                if (_orderService.IsUserInCourse(User.Identity.Name, episode.CourseId))
                {
                    byte[] file = System.IO.File.ReadAllBytes(FilePath);
                    return File(file, "application/force-download", FileName);
                }

            }
            return Forbid();
        }
        [HttpPost]
        [Route("CreateComment")]

        public IActionResult CreateComment(CourseComment comment)
        {
            comment.IsDelete = false;
            comment.CreateDate = DateTime.Now;
            comment.UserId = _userService.GetUserIdByUserName(User.Identity.Name);
            _service.AddComment(comment);

            return View("ShowComment", _service.CourseComment(comment.CourseId));
        }
        public IActionResult ShowComment(int Id, int pageId = 1)
        {
            return View(_service.CourseComment(Id, pageId));
        }

    }
}
