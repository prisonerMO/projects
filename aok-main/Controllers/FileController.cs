using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Linq.Expressions;

namespace AOK.Controllers
{
    public class FileController : Controller
    {

        private readonly IWebHostEnvironment _env;

        public FileController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public IActionResult Index()
        {
            return View();

        }

        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [HttpGet]
        public IActionResult FetchFile(string subFolder, string target)
        {
            if (User.Identity.IsAuthenticated)
            {
                var route = _env.ContentRootPath;
                var MainFilePath = Path.Combine(route, "Protected");
                var filePath = string.IsNullOrEmpty(subFolder) ? MainFilePath : Path.Combine(MainFilePath, subFolder);
                var fileName = target;
                var extension = Path.GetExtension(fileName);
                var contentType = "";
                new FileExtensionContentTypeProvider().TryGetContentType(extension, out contentType);
                if (string.IsNullOrEmpty(contentType))
                {
                    return NotFound();
                }


                var result = new PhysicalFileResult(Path.Combine(filePath, fileName), contentType);
                if (extension == ".html")
                {
                    result.FileDownloadName = target;
                }
                return result;
            }
            return NotFound();
        }


    }
}
