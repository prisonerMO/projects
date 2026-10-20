using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AOK.Models
{
    public class Game:DB_Object
    {
        [BsonElement("Name")]
        public string Name { get; set; } //game name

        [BsonElement("Description")]
        public string Description { get; set; } //game description

        [BsonElement("Link")]
        public string Link { get; set; } //game url

        [BsonElement("TitleImageUrl")]
        public string TitleImageUrl { get; set; }
    }
}
