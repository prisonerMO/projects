using MongoDB.Bson.Serialization.Attributes;
using SQLChecker.Models.Services;

namespace SQLChecker.Models
{
    public class Course: DB_Object
    {
        [BsonElement("CourseId")]
        public int CourseId { get; set; }

        [BsonElement("CourseIdStr")]
        public string CourseIdStr {  get; set; }

        [BsonElement("CourseName")]
        public string CourseName { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("Assignments")]
        public List<Assignment> Assignments { get; set; }
        public void Save()
        {
            DB_Services.Save(this);
        }

    }
}
