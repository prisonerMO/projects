using SQLChecker.Models;

namespace SQLChecker.Services
{

    public interface IAssignmentService
    {
        Task<Assignment?> GetAssignmentByIdAsync(string assignmentId);
        Task<List<Assignment>> GetAllAssignmentsAsync();
        Task<Assignment?> CreateAssignmentAsync(Assignment assignment);
        Task UpdateAssignmentAsync(Assignment assignment);
        Task DeleteAssignmentAsync(string assignmentId);
    }

}
