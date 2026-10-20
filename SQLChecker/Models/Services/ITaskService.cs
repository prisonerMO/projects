using SQLChecker.Models;

namespace SQLChecker.Services
{

    public interface ITaskService
    {
        Task<TaskDefinition?> GetTaskByIdAsync(string taskId);
        Task<List<TaskDefinition>> GetAllTasksAsync();
        Task<TaskDefinition?> CreateTaskAsync(TaskDefinition task);
        Task UpdateTaskAsync(TaskDefinition task);
        Task DeleteTaskAsync(string taskId);
    }

}
