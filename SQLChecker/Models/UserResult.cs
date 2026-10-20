using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SQLChecker.Models
{

    public class UserResult: DB_Object
    {

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("assignmentId")]
        public string AssignmentId { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [BsonElement("results")]
        public List<ResultRecord> Results { get; set; } = new();

        [BsonElement("score")]
        public int Score { get; set; }
    }

}
