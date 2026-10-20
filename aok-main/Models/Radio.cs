using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AOK.Models
{
    public class Radio: DB_Object
    {
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("RadioCall")]
        public string RadioCall { get; set; }

        [BsonElement("Channel")]
        public string Channel { get; set; }
    }
}
