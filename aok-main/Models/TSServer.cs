using MongoDB.Bson.Serialization.Attributes;

namespace AOK.Models
{
    public class TSServer : DB_Object
    {
        [BsonElement("Name")]
        public string Name { get; set; } //Team Speak Name

        [BsonElement("IP")]
        public string IP { get; set; } //Team Speak IP

        [BsonElement("Port")]
        public int Port { get; set; } //Team Speak Port

        [BsonElement("Password")]
        public string Password { get; set; } // Team Speak Password

    }
}
