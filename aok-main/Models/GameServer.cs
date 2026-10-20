using MongoDB.Bson.Serialization.Attributes;

namespace AOK.Models
{
    public class GameServer:DB_Object
    {
        [BsonElement("Name")]
        public string Name { get; set; } //Game Server Name

        [BsonElement("IP")]
        public string IP { get; set; } //Game Server IP

        [BsonElement("Port")]
        public int Port { get; set; } //Game Server Port

        [BsonElement("Password")]
        public string Password { get; set; } // Game Server Password

    }
}
