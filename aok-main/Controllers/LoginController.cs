using AOK.Models.Services;
using AOK.Models;
using AOK.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

namespace AOK.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("Index", new UserRegisterViewModel { Register = false });
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View("Index", new UserRegisterViewModel { Register = true });
        }


        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        [HttpPost, ValidateAntiForgeryToken]
        [Route("login")]
        public async Task<IActionResult> Login(UserRegisterViewModel model)
        {
            var user = await Task.Run(() => DB_Services.GetUserByUsername<User>(model.Username));

            if (user != null && Encryptor.VerifyPassword(model.Password, user.Password))
            {
                var userRoles = DB_Services.GetAllRoles(user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username),
                };

                if (userRoles != null && userRoles.Any())
                {
                    foreach (var role in userRoles)
                    {
                        if (!string.IsNullOrEmpty(role))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                    }
                }
                if (userRoles == null || !userRoles.Contains("Active"))
                {
                    ViewData["Error"] = "Käyttäjätili ei ole aktiivinen";
                    return View("Index", model);
                }
                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                user.LastLogin = DateTime.UtcNow;
                await Task.Run(() => user.Save());
                return RedirectToAction("Index", "Home");
            }

            ViewData["Error"] = "Väärä käyttäjätunnus tai salasana";
            return View("Index", model);
        }


        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            model.Register = true;

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            var unTrimmed = model.Username.Trim();
            var usernameN = model.Username.ToLowerInvariant();   
            var existingUser = DB_Services.GetResultFrom<User>("UsernameN", usernameN);
            if (existingUser != null)
            {
                ViewData["Error"] = "Käyttäjätunnus on jo käytössä.";
                return View("Index", model);
            }
            var email = model.Email.ToLowerInvariant();
            var existingEmail = DB_Services.GetResultFrom<User>("Email", email);
            if (existingEmail != null)
            {
                ViewData["Error"] = "Sähköposti on jo käytössä.";
                return View("Index", model);
            }

            var encryptedPassword = Encryptor.HashPassword(model.Password);

            var newUser = new User
            {
                Username = model.Username,
                UsernameN = usernameN,
                Email = email,
                Password = encryptedPassword,
                CreatedAt = DateTime.UtcNow,
                Roles = new List<string>()
            };
            await Task.Run(() => newUser.Save());

            return RedirectToAction("Login");
        }

        public IActionResult _LoginPartial()
        {
            return PartialView("_LoginPartial");
        }

        public IActionResult _RegisterPartial()
        {
            return PartialView("_RegisterPartial");
        }
    }
}
