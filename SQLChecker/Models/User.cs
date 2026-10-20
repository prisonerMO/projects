using SQLChecker.Models.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SQLChecker.Models
{
    public class User : DB_Object
    {
        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("UsernameN")]
        public string UsernameN { get; set; }

        [BsonElement("Course")]
        public string CourseId { get; set; }

        [BsonElement("Database")]
        public string Database { get; set; }

        [BsonElement("LastLogin")]
        public DateTime LastLogin { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("Roles")]
        public List<string> Roles { get; set; } = new List<string>();

        public void Save()
        {
            DB_Services.Save(this);
        }
    }
}
