using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SQLChecker.Models.Services;


namespace SQL.Controllers
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

        [HttpPost]
        public async Task<IActionResult> ImportUsers(IFormFile file, string courseId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Tiedosto puuttuu");

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".xlsx")
                return BadRequest("Vain .xlsx tiedostot sallittu");

            var users = new List<ImportedUserDto>();

            try
            {
                using var stream = file.OpenReadStream();
                using var document = SpreadsheetDocument.Open(stream, false);

                var workbookPart = document.WorkbookPart;
                var sheet = workbookPart.Workbook.Sheets.Elements<Sheet>().First();
                var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                var sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                var sharedStrings = workbookPart.SharedStringTablePart?.SharedStringTable;

                foreach (var row in sheetData.Elements<Row>())
                {
                    var cell = row.Elements<Cell>()
                                  .FirstOrDefault(c => c.CellReference != null &&
                                                       c.CellReference.Value.StartsWith("D"));

                    if (cell == null) continue;

                    var value = GetCellValue(cell, sharedStrings)?.Trim();

                    if (string.IsNullOrWhiteSpace(value))
                        continue;

                    if (value == "Käyttäjätunnus")
                        continue;

                    users.Add(new ImportedUserDto
                    {
                        Username = value,
                        CourseId = courseId
                    });
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Virhe tiedoston lukemisessa: {ex.Message}");
            }
        }
        private string GetCellValue(Cell cell, SharedStringTable sharedStrings)
        {
            if (cell == null)
                return "";

            var value = cell.InnerText;

            if (cell.DataType != null && cell.DataType == CellValues.SharedString)
            {
                return sharedStrings?.ElementAt(int.Parse(value)).InnerText ?? "";
            }

            return value;
        }

    }

}
