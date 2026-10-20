using SQLChecker.Models;
using SQLChecker.Models.Services;

namespace SQLChecker.Services
{

    public class TaskService : ITaskService
    {
        private readonly IConfiguration _configuration;
        public TaskService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<TaskDefinition?> GetTaskByIdAsync(string taskId)
        {
            return await Task.Run(() => DB_Services.GetObjectByIdStr<TaskDefinition>(taskId));
        }
        public async Task<List<TaskDefinition>> GetAllTasksAsync()
        {
            return await DB_Services.GetAllAsync<TaskDefinition>();
        }
        public async Task<TaskDefinition?> CreateTaskAsync(TaskDefinition task)
        {
            return await Task.Run(() =>
            {

                var newTask = new TaskDefinition   
                {
                    TaskId = task.TaskId,
                    Name = task.Name,
                    Description = task.Description,
                    ExpectedSql = task.ExpectedSql,
                    Checks = task.Checks,
                };

                newTask.Save();
                return newTask;
            });
        }


        public async Task UpdateTaskAsync(TaskDefinition task)
        {
            await Task.Run(() => task.Save());
        }

        public async Task DeleteTaskAsync(string assignmentId)
        {
            await Task.Run(() =>
            {
                var task = DB_Services.GetObjectByIdStr<TaskDefinition>(assignmentId);
                if (task != null)
                {
                    // Your delete logic
                }
            });
        }
    }

}
