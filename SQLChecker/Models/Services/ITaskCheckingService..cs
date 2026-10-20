using SQLChecker.Models;
using SQLChecker.Models.Services;

namespace SQLChecker.Services
{
    public interface ITaskCheckingService
    {
        Task<(bool Passed, string Error)> RunTask(TaskDefinition task, string studentSql);
        Task<List<CheckResult>> RunAllChecks(TaskDefinition task, string studentSql);
    }
}
