using AOK.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace AOK.ViewModels
{
    public class EventViewModel
    {

        [BsonElement("AllEvents")]
        public List<Event> AllEvents { get; set; } = new();

        [BsonElement("Communities")]
        public List<string> Communities { get; set; } = new();
    }
}
