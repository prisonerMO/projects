using SQLChecker.Models.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace SQLChecker.Models
{
    public class Database: DB_Object
    {

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("schema")]
        public string Schema { get; set; }

        [BsonElement("data")]
        public string Data { get; set; }
        public void Save()
        {
            DB_Services.Save(this);
        }
    }
}
