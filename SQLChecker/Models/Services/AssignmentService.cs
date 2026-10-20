using SQLChecker.Models;
using SQLChecker.Models.Services;

namespace SQLChecker.Services
{

    public class AssignmentService : IAssignmentService
    {
        private readonly IConfiguration _configuration;
        public AssignmentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Assignment?> GetAssignmentByIdAsync(string assignmentId)
        {
            return await Task.Run(() => DB_Services.GetObjectByIdStr<Assignment>(assignmentId));
        }
        public async Task<List<Assignment>> GetAllAssignmentsAsync()
        {
            return await DB_Services.GetAllAsync<Assignment>();
        }
        public async Task<Assignment> CreateAssignmentAsync(Assignment assignment)
        {
            return await Task.Run(() =>
            {

                var newAssignment = new Assignment
                {
                    Name = assignment.Name,
                    Description = assignment.Description,
                    CourseIdStr = assignment.CourseIdStr,
                    Tasks = assignment.Tasks,
                };

                newAssignment.Save();
                return newAssignment;
            });
        }


        public async Task UpdateAssignmentAsync(Assignment assignment)
        {
            await Task.Run(() => assignment.Save());
        }

        public async Task DeleteAssignmentAsync(string assignmentId)
        {
            await Task.Run(() =>
            {
                var assignment = DB_Services.GetObjectByIdStr<Assignment>(assignmentId);
                if (assignment != null)
                {
                    // Your delete logic
                }
            });
        }
    }

}
