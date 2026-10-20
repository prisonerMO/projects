using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SQLChecker.Models
{
    public class DB_Object
    {
            
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonElement("IdStr")]
        public string IdStr => _id.ToString();
    }
}

