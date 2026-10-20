using AOK.Models.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Xml.Linq;

namespace AOK.Models
{
    public class Event : DB_Object
    {

        [BsonElement("CreatedAt")]
        [BsonDateTimeOptions]
        public DateTime CreatedAt { get; set; }

        [BsonElement("CreatedBy")]
        public string CreatedBy { get; set; }

        [BsonElement("Published")]
        public bool Published { get; set; } = false;

        [BsonElement("TitleImageUrl")]
        public string TitleImageUrl { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Map")]
        public string Map { get; set; }

        [BsonElement("StartDate")]
        [BsonDateTimeOptions]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [BsonElement("EndDate")]
        [BsonDateTimeOptions]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [BsonElement("Duration")]
        public int Duration { get; set; } = 0;

        [BsonElement("SlottingOpenTime")]
        [BsonDateTimeOptions]
        public DateTime SlottingOpenTime { get; set; } = DateTime.Now;

        [BsonElement("LeaderSlottingOpenTime")]
        [BsonDateTimeOptions]
        public DateTime LeaderSlottingOpenTime { get; set; } = DateTime.Now;

        [BsonElement("SlottingCheckTime")]
        [BsonDateTimeOptions]
        public DateTime SlottingCheckTime { get; set; } = DateTime.Now;

        [BsonElement("PlanStartTime")]
        [BsonDateTimeOptions]
        public DateTime PlanStartTime { get; set; } = DateTime.Now;

        [BsonElement("PlanDuration")]
        public int PlanDuration { get; set; }

        [BsonElement("ModPreset")] 
        public string ModPreset { get; set; } = string.Empty;

        [BsonElement("MinPalyerrs")]
        public int MinPlayers { get; set; }

        [BsonElement("MaxPlayers")]
        public int MaxPlayers { get; set; }

        [BsonElement("MissionDate")]
        [BsonDateTimeOptions]
        public DateTime MissionDate { get; set; } = DateTime.Now;

        [BsonElement("Weather")]
        public string Weather { get; set; }

        [BsonElement("Rules")]
        public List<Rule> Rules { get; set; }

        [BsonElement("Situation")]
        public string Situation { get; set; }

        [BsonElement("Friendlies")]
        public string Friendlies { get; set; }

        [BsonElement("Enemies")]
        public string Enemies { get; set; }

        [BsonElement("Civilians")]
        public string Civilians { get; set; }

        [BsonElement("ROE")]
        public string ROE { get; set; }

        [BsonElement("Assests")]
        public string Assests { get; set; }

        [BsonElement("Mission")]
        public string Mission { get; set; }

        [BsonElement("Process")]
        public string Process { get; set; } 

        [BsonElement("Supports")]
        public string Supports { get; set; }
        
        [BsonElement("MedicalSettings")]
        public string MedicalSettings { get; set; }

        [BsonElement("Respawn")]
        public string Respawn { get; set; }

        [BsonElement("Comms")]
        public string Comms { get; set; }

        [BsonElement("Other")]
        public string Other { get; set; }

        //Others stuff
        [BsonElement("Game")]
        public Game Game { get; set; } = new Game();

        [BsonRequired]
        [BsonElement("Community")]
        public Community Community { get; set; } = new Community();

        [BsonElement("GameServer")]
        public GameServer GameServer { get; set; } = new GameServer();

        [BsonElement("TServer")]
        public TSServer TSServer { get; set; } = new TSServer();

        [BsonElement("RoleBoxes")]
        public List<RoleBox> RoleBoxes { get; set; } = new List<RoleBox>();

        [BsonIgnore] // optional if not stored in DB
        public List<User> Users { get; set; } = new List<User>();

        public void Save()
        {
            DB_Services.Save(this);
        }
    }
}
