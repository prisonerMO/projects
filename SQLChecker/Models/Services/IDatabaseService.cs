using SQLChecker.Models;

namespace SQLChecker.Services
{

    public interface IDatabaseService
    {
        Task<Database?> GetDatabaseByIdAsync(string databaseId);
        Task<List<Database>> GetAllDatabasesAsync();
        Task<Database?> CreateDatabaseAsync(Database database);
        Task UpdateDatabaseAsync(Database database);
        Task DeleteDatabaseAsync(string databaseId);

        Task<Dictionary<string, List<string>>> GetSchemaAsync(string databaseName);

        Task<List<List<string>>> ExecuteAsync(string query);
    }

}
