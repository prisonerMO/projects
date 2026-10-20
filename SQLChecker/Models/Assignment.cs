using MongoDB.Bson.Serialization.Attributes;
using SQLChecker.Models.Services;
using System.Security.Policy;

namespace SQLChecker.Models
{

    public class Assignment:DB_Object
    {

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("courseId")]
        public string CourseIdStr { get; set; }

        [BsonElement("tasks")]
        public List<TaskDefinition> Tasks { get; set; } = new();
        public void Save()
        {
            DB_Services.Save(this);
        }
    }

}
