using MongoDB.Bson.Serialization.Attributes;

namespace AOK.Models
{
    public class Player:DB_Object
    {
        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("Nickname")]
        public string Nickname { get; set; }

        [BsonElement("Community")]
        public string Community { get; set; }
    }
}
