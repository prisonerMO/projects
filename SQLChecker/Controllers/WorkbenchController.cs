using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using SQLChecker.Models;
using SQLChecker.Models.Services;
using SQLChecker.Models.ViewModels;
using SQLChecker.Services;
using SQLChecker.ViewModels;

namespace SQLChecker.Controllers
{
    public class WorkbenchController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IAssignmentService _assignmentService;
        private readonly IDatabaseService _sqlExecution;
        private readonly IConfiguration _configuration;

        public WorkbenchController(ICourseService courseService, IAssignmentService assignmentService, IDatabaseService sqlExecution, IConfiguration configuration)           
        {
            _courseService = courseService;
            _assignmentService = assignmentService;
            _sqlExecution = sqlExecution;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string courseId)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound();

            var user = DB_Services.GetUserByUsername<User>(User.Identity.Name);
            if (user == null)
            {
                TempData["Error"] = "Virheellinen käyttäjä tai kurssi";
                return RedirectToAction("Index", "Home");
            }

            var allAssignments = await _assignmentService.GetAllAssignmentsAsync();
            var assignments = allAssignments?
                .Where(a => a.CourseIdStr == course.IdStr)
                .ToList() ?? new();

            var schema = await _sqlExecution.GetSchemaAsync(user.Database); 

            var vm = new WorkbenchViewModel
            {
                User = user,
                Course = course,
                Assignments = assignments,
                DbSchema = schema,
                SelectedDatabase = user.Database
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> RunQuery([FromBody] RunQueryRequestViewModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Sql))
                return BadRequest("SQL ei voi olla tyhjä");

            if (string.IsNullOrWhiteSpace(request.DatabaseName))
                return BadRequest("Käyttäjällä ei ole valittua tietokantaa");

            try
            {
                var finalSql = $"USE `{request.DatabaseName}`; {request.Sql}";

                var raw = await _sqlExecution.ExecuteAsync(finalSql);

                var result = new QueryResultViewModel
                {
                    Columns = raw.FirstOrDefault() ?? new(),
                    Rows = raw.Skip(1).ToList()
                };

                return PartialView("_QueryResults", result);
            }
            catch (Exception ex)
            {
                return PartialView("_QueryResults", new QueryResultViewModel
                {
                    Error = ex.Message
                });
            }
        }
    }
}