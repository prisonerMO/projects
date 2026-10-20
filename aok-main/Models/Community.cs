using AOK.Models.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace AOK.Models
{
    public class Community : DB_Object
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TitleImageUrl { get; set; }

        public static Community Create(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Yhteisön nimi on pakollinen.");

            return new Community
            {
                _id = MongoDB.Bson.ObjectId.GenerateNewId(),
                Name = name,
                Description = description
            };
        }
        public void Save()
        {
            DB_Services.Save(this);
        }
    }
}
