using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SQLChecker.Models;
using static SQLChecker.Models.Services.Encryptor;

namespace SQLChecker.Controllers
{

    [ApiController]
    [Route("api")]
    public class EncryptionController : Controller
    {
        private readonly IConfiguration _configuration;

        // Single constructor with all dependencies
        public EncryptionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("encrypt")]
        public string EncryptUser([FromBody] LoginPayload payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var secret = _configuration["CrypterSecret"];
            var crypter = new UserTokenEncryptor(secret);
            return crypter.EncryptPayload(json);
        }


        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                status = "OK",
                message = "Health-check working",
                time = DateTime.UtcNow
            });
        }

    }

}
