using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class GroupeController : Controller
    {
        ICourseService _courseService;
        public GroupeController(ICourseService coursService)
        {
            _courseService = coursService;
        }
        public IActionResult Index()
        {
            var Groupe = _courseService.GetAllGroup();
            return View(Groupe);
        }
        [Route("adminpanel/Groupe/CreateGroup/{Id?}")]
        public IActionResult CreateGroup(int? Id)
        {
            return View();
        }
        [HttpPost]
        [Route("adminpanel/Groupe/CreateGroup/{Id?}")]
        public IActionResult CreateGroup(int? Id, CourseGroup courseGroup)
        {
            if (!ModelState.IsValid && courseGroup == null)
                return View(courseGroup);
            if (Id != null)
            {
                courseGroup.ParentId = Id;
            }
            _courseService.AddGroupe(courseGroup);

            return Redirect("/adminpanel/Groupe");
        }
        [Route("adminpanel/Groupe/EditGroup/{Id}")]
        public IActionResult EditGroup(int Id)
        {
            return View(_courseService.GetByCourseGroupId(Id));
        }

        [HttpPost]
        [Route("adminpanel/Groupe/EditGroup/{Id}")]
        public IActionResult EditGroup(CourseGroup courseGroup)
        {
            if (!ModelState.IsValid && courseGroup == null)
                return View(courseGroup);
            _courseService.UpdateGroupe(courseGroup);

            return Redirect("/adminpanel/Groupe");
        }
    }

}
