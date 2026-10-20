using AOK.Models.Services;
using AOK.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AOK.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;

namespace AOK.Controllers
{
    public class AccountController : Controller
    {
        [Authorize]
        public IActionResult Settings()
        {
            string username = User.Identity?.Name;

            var user = DB_Services.GetUserByUsername<User>(username);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var communities = DB_Services.GetAll<Community>()
                .Select(c => new SelectListItem
                {
                    Value = c._id.ToString(),
                    Text = c.Name
                }).ToList();
            var community = communities.FirstOrDefault(c => c.Value == user.Community.ToString());
            string communityText = community != null ? community.Text : "tuntematon";
            var model = new AccountViewModel
            {
                Username = user.Username,
                Nickname = user.Nickname,
                Community = user.Community.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                SelectedCommunity = communityText,
                Communities = communities
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateAccount(AccountViewModel model)
        {
            string username = User.Identity?.Name;
            var user = DB_Services.GetUserByUsername<User>(username);
            var communities = DB_Services.GetAll<Community>()
                .Select(c => new SelectListItem
                {
                    Value = c._id.ToString(),
                    Text = c.Name
                }).ToList();
            model.Communities = communities;
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {


                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View("Settings", model);
            }
            if (ObjectId.TryParse(model.Community, out var communityId))
            {
                user.Community = communityId;
            }
            else
            {
                ModelState.AddModelError("Community", "Invalid community selected.");
                return View("Settings", model);
            }

            user.Nickname = model.Nickname;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.Save();

            TempData["SuccessMessage"] = "Tietojen päivitys onnistui!";
            return RedirectToAction("Settings");
        }

        public IActionResult _ChangePasswordPartial()
        {
            return PartialView("_ChangePasswordPartial");
        }
        [HttpPost]
        [Authorize]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string username = User.Identity?.Name;
            var user = DB_Services.GetUserByUsername<User>(username);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }
            if (!Encryptor.VerifyPassword(model.OldPassword, user.Password))
            {
                ModelState.AddModelError("OldPassword", "Nykyinen salasana ei ole oikea.");
                return View(model);
            }
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Salasanat eivät täsmää.");
                return View(model);
            }
            user.Password = Encryptor.HashPassword(model.NewPassword);
            user.Save();

            TempData["SuccessMessage"] = "Salasana vaihdettu onnistuneesti!";
            return RedirectToAction("Settings");
        }

    }
}
