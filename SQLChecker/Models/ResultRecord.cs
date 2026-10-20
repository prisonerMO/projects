using MongoDB.Bson.Serialization.Attributes;

namespace SQLChecker.Models
{

    public class ResultRecord
    {
        [BsonElement("taskId")]
        public int TaskId { get; set; }

        [BsonElement("passed")]
        public bool Passed { get; set; }

        [BsonElement("studentSql")]
        public string StudentSql { get; set; }

        [BsonElement("errorMessage")]
        public string ErrorMessage { get; set; }
    }

}
