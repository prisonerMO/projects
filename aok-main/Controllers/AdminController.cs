using AOK.Models;
using AOK.Models.Services;
using AOK.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AOK.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index(AdminViewModel model)
        {

            if (User.IsInRole("Admin"))
            {
                if (model == null)
                {
                    return RedirectToAction("Home","Index");
                }

                return View(model);
            }
            return Unauthorized();
        }

        public IActionResult Users()
        {
            var model = new AdminViewModel
            {
                Type = 0, // users myoh
                Users = DB_Services.GetAll<User>(),
            };
            return View("Index", model);
        }

        public IActionResult Communities()
        {
            var model = new AdminViewModel
            {
                Type = 1,
                Communities = DB_Services.GetAll<Community>(),
            };
            return View("Index", model);
        }

        public IActionResult Games()
        {
            var model = new AdminViewModel
            {
                Type = 2,
                Games = DB_Services.GetAll<Game>(),
            };
            return View("Index", model);
        }

    }
}