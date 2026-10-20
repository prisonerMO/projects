using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SQLChecker.Models;
using SQLChecker.Services;
using System.Diagnostics;
using SQLChecker.Models.Services;

namespace SQLChecker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICourseService _courseService;

        public HomeController(ILogger<HomeController> logger, ICourseService courseService)
        {
            _logger = logger;
            _courseService = courseService;
        }

        public async Task<IActionResult> Index()
        {
            //if (!User.Identity.IsAuthenticated)
            //{
            //    return Redirect("/error");
            //}

            var courses = await _courseService.GetAllCoursesAsync();

            return View(courses);
        }

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
