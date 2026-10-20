using AOK.Models.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace AOK.Models
{
    public class Rule:DB_Object
    {
        [BsonElement("Title")]
        public string Title { get; set; }
        [BsonElement("Description ")]
        public string Description { get; set; }
        [BsonElement("Order")]
        public int Order { get; set; }

        public void Save()
        {
            DB_Services.Save(this);
        }
    }
}
