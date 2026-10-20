using SQLChecker.Models;

namespace SQLChecker.Services
{
    public interface ICourseService
    {
        Task<Course?> GetCourseByIdAsync(string courseId);
        Task<Course?> GetCourseObjectByIdStrAsync(string courseIdStr);
        Task<List<Course>> GetAllCoursesAsync();
        Task<Course> CreateCourseAsync(Course course);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(string courseId);
    }
}