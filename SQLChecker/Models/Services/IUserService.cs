// Services/IUserService.cs
using SQLChecker.Models;

namespace SQLChecker.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<List<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string username);
    }
}