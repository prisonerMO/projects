using MongoDB.Bson.Serialization.Attributes;

namespace AOK.Models
{
    public class RoleBox:DB_Object
    {

        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;
        
        [BsonElement("Description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("Order")]
        public int Order { get; set; } = 0;
        
        [BsonElement("Roles")]
        public List<Role> Roles { get; set; } = new List<Role>();

    }
}
