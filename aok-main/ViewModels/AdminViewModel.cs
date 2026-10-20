using MongoDB.Bson.Serialization.Attributes;
using AOK.Models;

namespace AOK.ViewModels
{
    public class AdminViewModel
    {
        [BsonElement("Type")]
        public int Type { get; set; } = 0;

        [BsonElement("Communities")]
        public List<Community> Communities { get; set; } = new();

        [BsonElement("Games")]
        public List<Game> Games { get; set; } = new();

        [BsonElement("Users")]
        public List<User> Users { get; set; } = new();
    }
}
