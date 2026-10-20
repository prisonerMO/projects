using SQLChecker.Models;
using SQLChecker.Models.Services;

namespace SQLChecker.Services
{
    public class CourseService : ICourseService
    {
        private readonly IConfiguration _configuration;

        public CourseService(IConfiguration configuration)
        {
            _configuration = configuration;
            Console.WriteLine($"CrypterSecret loaded: {configuration["CrypterSecret"] != null}");
        }

        public async Task<Course?> GetCourseByIdAsync(string courseId)
        {
            return await Task.Run(() => DB_Services.GetCourseByIdStr<Course>(courseId));
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await DB_Services.GetAllAsync<Course>();
        }

        public async Task<Course?> GetCourseObjectByIdStrAsync(string courseId)
        {
            return await Task.Run(() => DB_Services.GetObjectByIdStr<Course>(courseId));
        }
        //public async Task<Course> CreateCourseAsync(Course course)
        //{
        //    return await Task.Run(() =>
        //    {
        //        // Validate
        //        var existingUser = DB_Services.GetUserByUsername<User>(user.Username);
        //        if (existingUser != null)
        //            throw new Exception("Käyttäjänimi on jo käytössä.");

        //        var course = DB_Services.GetCourseByIdStr<Course>(user.CourseId);
        //        if (course == null)
        //            throw new Exception("Kurssia ei löydy.");

        //        // Get secret and validate
        //        var secret = _configuration["CrypterSecret"];
        //        if (string.IsNullOrEmpty(secret))
        //        {
        //            throw new InvalidOperationException("CrypterSecret is not configured in appsettings.json");
        //        }

        //        var crypter = new UserNameCrypter(secret);
        //        var encryptedUsername = crypter.Encrypt(user.Username + "#" + user.CourseId);

        //        // Create new user with encrypted username
        //        var newUser = new User
        //        {
        //            Username = encryptedUsername,          // Encrypted version
        //            UsernameN = user.Username,             // Plain username for debug
        //            CourseId = user.CourseId,
        //            CreatedAt = DateTime.UtcNow,
        //            LastLogin = DateTime.UtcNow,
        //            Roles = user.Roles ?? new List<string>()
        //        };

        //        newUser.Save();
        //        return newUser;
        //    });
        //}

        public async Task<Course> CreateCourseAsync(Course course)
        {
            return await Task.Run(() =>
            {
                
                var newCourse = new Course
                {
                    CourseId = course.CourseId,
                    CourseIdStr = course.CourseIdStr,
                    CourseName = course.CourseName,
                    CreatedAt = DateTime.UtcNow,
                    Assignments = course.Assignments ?? new List<Assignment>()
                };

                newCourse.Save();
                return newCourse;
            });
        }


        public async Task UpdateCourseAsync(Course course)
        {
            await Task.Run(() => DB_Services.Update<Course>(course));
        }

        public async Task DeleteCourseAsync(string courseId)
        {
            await Task.Run(() =>
            {
                var course = DB_Services.GetCourseByIdStr<Course>(courseId);
                if (course != null)
                {
                    // Your delete logic
                }
            });
        }
    }
}