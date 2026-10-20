using Microsoft.Extensions.Configuration.UserSecrets;
using SQLChecker.Models;
using SQLChecker.Models.Services;
using System.Security.Cryptography;
using System.Text;

namespace SQLChecker.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabaseService _databaseService;
        public UserService(IConfiguration configuration, IDatabaseService databaseService)
        {
            _configuration = configuration;
            _databaseService = databaseService;
            Console.WriteLine($"CrypterSecret loaded: {configuration["CrypterSecret"] != null}");
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await Task.Run(() => DB_Services.GetUserByUsername<User>(username));
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await DB_Services.GetAllAsync<User>();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await Task.Run(async () =>
            {
                var secret = _configuration["CrypterSecret"];
                if (string.IsNullOrEmpty(secret))
                    throw new InvalidOperationException("CrypterSecret is missing");

                string usernameHash = Encryptor.ComputeUserHash(
                    user.Username,
                    user.CourseId,
                    secret
                );

                var existing = DB_Services.GetUserByUsername<User>(usernameHash);
                if (existing != null)
                    throw new Exception("Käyttäjä on jo olemassa tällä kurssilla.");

                var course = DB_Services.GetCourseByIdStr<Course>(user.CourseId);
                if (course == null)
                    throw new Exception("Kurssia ei löydy.");

                var databaseName = $"db_{usernameHash[..12]}";

                await _databaseService.CreateDatabaseAsync(new Database
                {
                    Name = databaseName,
                    CreatedAt = DateTime.UtcNow
                });

                var newUser = new User
                {
                    Username = usernameHash,
                    UsernameN = user.Username,
                    CourseId = user.CourseId,
                    Database = databaseName,
                    CreatedAt = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                    Roles = user.Roles ?? new List<string>()
                };

                newUser.Save();
                return newUser;
            });
        }

        public async Task UpdateUserAsync(User user)
        {
            await Task.Run(() => user.Save());
        }

        public async Task DeleteUserAsync(string username)
        {
            await Task.Run(() =>
            {
                var user = DB_Services.GetUserByUsername<User>(username);
                if (user != null)
                {
                    // Your delete logic
                }
            });
        }

        public async Task CreateDatabase(string databaseName)
        {

            await Task.Run(async () => {
                try
                {
                    var newDatabase = new Database();
                    var createdDatabase = await _databaseService.CreateDatabaseAsync(newDatabase);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            });

        }
    }
}