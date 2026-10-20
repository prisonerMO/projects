using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AOK.Models
{
    public class Role: DB_Object
    {

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Player")]
        public Player? Player { get; set; }

        [BsonElement("Radio")]
        public List<Radio> Radios { get; set; } = new List<Radio>();

        [BsonElement("RadioCall")]
        public string RadioCall { get; set; } = "";

        [BsonElement("Order")]
        public int Order { get; set; } = -1;

        [BsonElement("IsLeaderSlot")]
        public bool IsLeaderSlot { get; set; } = false;


        public Role Clone()
        {
            var clonedRole = new Role
            {
                _id = ObjectId.GenerateNewId(),
                Name = this.Name,
                RadioCall = this.RadioCall,
                Order = this.Order,
                IsLeaderSlot = this.IsLeaderSlot,
                Radios = new List<Radio>(this.Radios)
            };
            return clonedRole;
        }
    }

}
