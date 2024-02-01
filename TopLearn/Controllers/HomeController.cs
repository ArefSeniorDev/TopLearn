using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Models;

namespace TopLearn.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private ICourseService _service;
        public HomeController(ILogger<HomeController> logger, ICourseService service)
        {
            _logger = logger;
            _service = service;
        }

        public IActionResult Index()
        {
            return View(_service.GetCourse());
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}