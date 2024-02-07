using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpCompress.Archives;
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
        public IActionResult ShowCourse(int Id, int episode = 0)
        {
            var Sincourse = _service.GetCourseForShow(Id);
            if (Sincourse == null)
            {
                return NotFound();
            }
            if (episode != 0 && User.Identity.IsAuthenticated)
            {
                if (Sincourse.CourseEpisodes.All(e => e.EpisodeId != episode))
                {
                    return NotFound();
                }

                if (!Sincourse.CourseEpisodes.First(e => e.EpisodeId == episode).IsFree)
                {
                    if (!_orderService.IsUserInCourse(User.Identity.Name, Id))
                    {
                        return NotFound();
                    }
                }

                var ep = Sincourse.CourseEpisodes.First(e => e.EpisodeId == episode);
                ViewBag.Episode = ep;
                string filePath = "";
                string checkFilePath = "";
                if (ep.IsFree)
                {
                    filePath = "/CourseOnline/" + ep.EpisodeFileName.Replace(".rar", ".mp4");
                    checkFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CourseOnline",
                        ep.EpisodeFileName.Replace(".rar", ".mp4"));
                }
                else
                {
                    filePath = "/CourseFilesOnline/" + ep.EpisodeFileName.Replace(".rar", ".mp4");
                    checkFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CourseFilesOnline",
                        ep.EpisodeFileName.Replace(".rar", ".mp4"));
                }


                if (!System.IO.File.Exists(checkFilePath))
                {
                    string targetPath = Directory.GetCurrentDirectory();
                    if (ep.IsFree)
                    {
                        targetPath = System.IO.Path.Combine(targetPath, "wwwroot/CourseOnline");
                    }
                    else
                    {
                        targetPath = System.IO.Path.Combine(targetPath, "wwwroot/CourseFilesOnline");
                    }

                    string rarPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course_ep",
                        ep.EpisodeFileName);
                    var archive = ArchiveFactory.Open(rarPath);

                    var Entries = archive.Entries.OrderBy(x => x.Key.Length);
                    foreach (var en in Entries)
                    {
                        if (Path.GetExtension(en.Key) == ".mp4")
                        {
                            en.WriteTo(System.IO.File.Create(Path.Combine(targetPath, ep.EpisodeFileName.Replace(".rar", ".mp4"))));
                        }
                    }
                }

                ViewBag.filePath = filePath;
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
        public IActionResult VoteCourse(int Id)
        {
            return PartialView(_service.GetCourseVote(Id));
        }
        [Authorize]
        public IActionResult AddVote(int Id, bool vote)
        {
            int UserId = _userService.GetUserIdByUserName(User.Identity.Name);
            _service.AddVote(Id, UserId, vote);
            return PartialView("VoteCourse", _service.GetCourseVote(Id));

        }
    }
}
