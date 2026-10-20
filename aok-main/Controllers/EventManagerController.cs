using AOK.Models.Services;
using AOK.Models;
using AOK.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace AOK.Controllers
{
    [Authorize(Roles = "Admin,PowerUser")]
    public class EventManagerController : Controller
    {
        public IActionResult Index(EventManagerViewModel model)
        {
            return View(model);
        }

        public async Task<IActionResult> All()
        {
            string username = User.Identity?.Name;
            var user = DB_Services.GetUserByUsername<User>(username);
            if (user == null)
            {
                return NotFound();
            }
            var userCommunity = user.Community.ToString();
            var model = new EventManagerViewModel
            {
                Type = 0,
                CurrentUser = username,
                CurrentUserCommunity = userCommunity,
                AllEvents = await DB_Services.GetAllAsync<Event>(),
            };
            return View("Index", model);
        }
        [Authorize(Roles = "Admin,PowerUser")]
        public async Task<IActionResult> Create()
        {

            string username = User.Identity?.Name;
            var model = new EventManagerViewModel
            {
                CurrentUser = username,
                Type = 1,
                Event = new Event()
            };
            return View("Index", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,PowerUser")]
        public async Task<IActionResult> Modify(ObjectId id)
        {
            string username = User.Identity?.Name;
            var selectedEvent = await DB_Services.GetObjectById<Event>(id);
            if (selectedEvent == null)
            {
                return NotFound();
            }
            var model = new EventManagerViewModel
            {
                SelectedEvent = selectedEvent.IdStr,
                CurrentUser = username,
                Type = 2,
                Event = selectedEvent,
            };          

            return View("Index", model);
        }
        [Authorize(Roles = "Admin,PowerUser")]
        public async Task<IActionResult> Show(ObjectId id)
        {
            var selectedEvent = await DB_Services.GetObjectById<Event>(id);
            if (selectedEvent == null)
            {
                return NotFound();
            }
            var model = new EventManagerViewModel
            {
                Type = 2,
                Event = selectedEvent
            };
            return View("Index", model);
        }
        [Authorize(Roles = "Admin,PowerUser")]
        public IActionResult GameServers()
        {
            var model = new EventManagerViewModel
            {
                Type = 3
            };
            return View("Index", model);
        }
        [Authorize(Roles = "Admin,PowerUser")]
        public IActionResult TSServers()
        {
            var model = new EventManagerViewModel
            {
                Type = 4
            };
            return View("Index", model);
        }
        [Authorize(Roles = "Admin,PowerUser")]
        public IActionResult Roles()
        {
            var model = new EventManagerViewModel
            {
                Type = 5
            };
            return View("Index", model);
        }
        [Authorize(Roles = "Admin,PowerUser")]
        public IActionResult Rules()
        {
            var model = new EventManagerViewModel
            {
                Type = 6
            };
            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,PowerUser")]
        public async Task<IActionResult> Delete(ObjectId id)
        {
            var selectedEvent = await DB_Services.GetObjectById<Event>(id);
            if (selectedEvent == null)
            {
                return NotFound();
            }
            DB_Services.Delete<Event>(selectedEvent._id);
            return RedirectToAction("All");
        }




        [HttpPost]
        [RequestSizeLimit(2 * 1024 * 1024)] // 2MB max
        public async Task<IActionResult> UploadTitleImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images/games");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"/uploads/images/games/{fileName}";
            return Ok(new { imageUrl });
        }
    }
}
