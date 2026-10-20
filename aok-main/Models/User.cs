using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using AOK.Models.Services;

namespace AOK.Models
{
    public class User: DB_Object
    {
        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("UsernameN")]
        public string UsernameN { get; set; }

        [BsonElement("Nickname")]
        public string Nickname { get; set; }

        [BsonElement("Community")]
        public ObjectId Community { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("FirstName")]
        public string FirstName { get; set; }

        [BsonElement("LastName")]
        public string LastName { get; set; }

        [BsonElement("LastLogin")]
        public DateTime LastLogin { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("Roles")]
        public List<string> Roles { get; set; } = new List<string>();

        public void Save()
        {
            DB_Services.Save(this);
        }
    }
}