using MongoDB.Bson.Serialization.Attributes;
using SQLChecker.Models;

namespace SQLChecker.ViewModels
{
    public class AdminViewModel
    {
        [BsonElement("Type")]
        public int Type { get; set; } = 0;

        [BsonElement("Users")]
        public List<User> Users { get; set; } = new();

        [BsonElement("Courses")]
        public List<Course> Courses { get; set; } = new();

        [BsonElement("Assignments")]
        public List<Assignment> Assignments { get; set; } = new();

        [BsonElement("Tasks")]
        public List<TaskDefinition> Tasks { get; set; } = new();

        public CreateUserViewModel CreateUser { get; set; } = new CreateUserViewModel();

        [BsonElement("Databases")]
        public List<Database> Databases { get; set; } = new();

    }
}
