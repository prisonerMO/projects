using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SQLChecker.Models;
using SQLChecker.Models.Services;
using System.Security.Claims;

namespace SQLChecker.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;
        public LoginController(IConfiguration configuration, ILogger<LoginController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("start")]
        public async Task<IActionResult> Login(string id, string course, long ts)
        {
            if (string.IsNullOrWhiteSpace(id) ||
                string.IsNullOrWhiteSpace(course) ||
                ts <= 0)
            {
                TempData["Error"] = "Virheellinen linkki";
                return RedirectToAction("Index", "Home");
            }

            if (!Encryptor.IsTimestampValid(ts))
            {
                TempData["Error"] = "Linkki on vanhentunut";
                return RedirectToAction("Index", "Home");
            }

            string secret = _configuration["CrypterSecret"];
            if (string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("CRYPTERSECRET missing");

            string userHash = Encryptor.ComputeUserHash(
                id,
                course,
                secret
            );

            var user = DB_Services.GetUserByUsername<User>(userHash);
            if (user == null)
            {
                TempData["Error"] = "Virheellinen käyttäjä tai kurssi";
                return RedirectToAction("Index", "Home");
            }

            var roles = DB_Services.GetAllRoles(user);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.NameIdentifier, user.UsernameN),
        new Claim("CourseId", user.CourseId)
    };

            if (roles != null)
            {
                foreach (var role in roles.Where(r => !string.IsNullOrWhiteSpace(r)))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                    IsPersistent = true
                }
            );

            user.LastLogin = DateTime.UtcNow;
            await Task.Run(() => user.Save());

            TempData["Success"] = "Kirjautuminen onnistui!";
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }





    }
}
