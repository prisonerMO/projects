using AOK.Models.Services;
using AOK.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using AOK.ViewModels;
using System.Threading.Channels;
using System;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;

namespace AOK.Controllers
{
    public class EventsController : Controller
    {

        [Route("")]
        [Route("Events")]
        public IActionResult Index()
        {
            var model = new EventViewModel();
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Show(string id)
        {
            if (ObjectId.TryParse(id, out ObjectId objectId))
            {
                var eventModel = await DB_Services.GetObjectById<Event>(objectId);

                if (eventModel == null)
                {
                    return NotFound();
                }
                return View(eventModel);
            }
            else
            {
                return BadRequest("Invalid event ID.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Signup(string roleId, string eventId)
        {
            var currentUser = User.Identity?.Name?.ToLower();
            if (string.IsNullOrEmpty(currentUser))
                return Unauthorized();

            if (!ObjectId.TryParse(eventId, out ObjectId eventObjectId))
                return BadRequest("Invalid event ID.");

            var gameEvent = await DB_Services.GetObjectById<Event>(eventObjectId);
            if (gameEvent == null)
                return NotFound("Event not found.");

            var alreadySigned = gameEvent.RoleBoxes
                .SelectMany(rb => rb.Roles)
                .Any(r => r.Player?.Username?.ToLower() == currentUser);

            if (alreadySigned)
                return BadRequest("You have already signed up.");

            var role = gameEvent.RoleBoxes
                .SelectMany(rb => rb.Roles)
                .FirstOrDefault(r => r.IdStr == roleId);

            if (role == null || !string.IsNullOrEmpty(role.Player?.Username))
                return BadRequest("Slot not available.");
            var user = await Task.Run(() => DB_Services.GetUserByUsername<User>(currentUser));
            var userCommunity = await DB_Services.GetObjectById<Community>(user.Community);

            var newPlayer = new Player
            {
                Username = user.UsernameN,
                Nickname = user.Nickname,
                Community = userCommunity.Name,
            };
            role.Player = newPlayer;

            gameEvent.Save();

            return RedirectToAction("Show", new { id = eventId });
        }


        [HttpPost]
        public async Task<IActionResult> CancelSignup(string roleId, string eventId)
        {
            var currentUser = User.Identity?.Name?.ToLower();
            if (string.IsNullOrEmpty(currentUser))
                return Unauthorized();

            if (!ObjectId.TryParse(eventId, out ObjectId eventObjectId))
                return BadRequest("Invalid event ID.");

            var gameEvent = await DB_Services.GetObjectById<Event>(eventObjectId);
            if (gameEvent == null)
                return NotFound("Event not found.");

            var roleBox = gameEvent.RoleBoxes
                .FirstOrDefault(rb => rb.Roles.Any(r => r.IdStr == roleId));
            if (roleBox == null)
                return NotFound("Role not found.");

            var role = roleBox.Roles.FirstOrDefault(r => r.IdStr == roleId);
            if (role == null || role.Player?.Username?.ToLower() != currentUser)
                return Unauthorized("You can only cancel your own signup.");

            role.Player = null;
            gameEvent.Save();

            return View("Show", gameEvent);
        }

    }
}
