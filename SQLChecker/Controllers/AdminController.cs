using Microsoft.AspNetCore.Mvc;
using SQLChecker.Models;
using SQLChecker.Models.Services;
using SQLChecker.ViewModels;

namespace SQLChecker.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Users()
        {
            var model = new AdminViewModel
            {
                Type = 0,
                Users = DB_Services.GetAll<User>(),
            };
            return View("Index", model);
        }
        public ActionResult Courses()
        {
            var model = new AdminViewModel
            {
                Type = 1,
                Courses = DB_Services.GetAll<Course>(),
            };
            return View("Index", model);
        }

        public ActionResult Assignments()
        {
            var model = new AdminViewModel
            {
                Type = 2,
                Assignments = DB_Services.GetAll<Assignment>(),
            };
            return View("Index", model);
        }
        public ActionResult Tasks()
        {
            var model = new AdminViewModel
            {
                Type = 3,
                Tasks = DB_Services.GetAll<TaskDefinition>(),
            };
            return View("Index", model);
        }
        public ActionResult Databases() 
        {
            var model = new AdminViewModel
            {
                Type = 4,
                Databases = DB_Services.GetAll<Database>(),
            };
            return View("Index", model);
        }

    }
}
