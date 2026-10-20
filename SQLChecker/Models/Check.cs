using MongoDB.Bson.Serialization.Attributes;

namespace SQLChecker.Models
{

    public class Check
    {
        [BsonElement("type")]
        public string Type { get; set; }  // e.g. "Contains", "CompareResult"

        [BsonElement("value")]
        public string Value { get; set; } // optional; used for checks like Contains("pg-13")
    }

}
