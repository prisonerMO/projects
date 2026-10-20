using MongoDB.Bson.Serialization.Attributes;
using SQLChecker.Models.Services;

namespace SQLChecker.Models
{

    public class TaskDefinition: DB_Object 
    {
        [BsonElement("taskId")]
        public int TaskId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("expectedSql")]
        public string ExpectedSql { get; set; }

        [BsonElement("hint")]
        public string Hint { get; set; }

        [BsonElement("checks")]
        public List<Check> Checks { get; set; } = new();

        public void Save()
        {
            DB_Services.Save(this);
        }
    }

}
