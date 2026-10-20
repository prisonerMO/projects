using MongoDB.Bson.Serialization.Attributes;
using AOK.Models;
using MongoDB.Bson;
using Microsoft.AspNetCore.Components.Forms;

namespace AOK.ViewModels
{
    public class EventManagerViewModel
    {
        [BsonElement("CurrentUser")]
        public string CurrentUser { get; set; }

        [BsonElement("CurrentUserCommunity")]
        public string CurrentUserCommunity { get; set; }

        [BsonElement("Type")]
        public int Type { get; set; } = 0;

        [BsonElement("AllEvents")]
        public List<Event> AllEvents { get; set; } = new();

        [BsonElement("Event")]
        public Event Event { get; set; }

        [BsonElement("SelectedEvent")]
        public string SelectedEvent { get; set; }

        [BsonElement("Games")]
        public List<Game> Games { get; set; } = new();

        [BsonElement("TSServers")]
        public List<TSServer> TSServers { get; set; } = new();

        [BsonElement("GameServers")]
        public List<GameServer> GameServers { get; set; } = new();

        [BsonElement("Communities")]
        public List<Community> Communities { get; set; } = new();

        [BsonElement("Roles")]
        public List<Role> Roles { get; set; } = new();

        [BsonElement("RoleBoxes")]
        public List<RoleBox> RoleBoxes { get; set; } = new();

        [BsonElement("Radios")]
        public List<Radio> Radios { get; set; } = new();

        public Community CommunityInput { get; set; } = new Community();
        public GameServer GameServerInput { get; set; } = new GameServer();
        public TSServer TSServerInput { get; set; } = new TSServer();
        public Game GameInput { get; set; } = new Game();
        public bool IsRoleBoxModalOpen = false;
        public bool ShowDataBox = false;
        public RoleBox NewRoleBox { get; set; } = new();
        public RoleBox? SelectedBox = null;
        public List<RoleBox> RoleBoxTemplates = new List<RoleBox>();
        public List<string> Pictures { get; set; } = new();
        public List<User> OwnCommunityUsers { get; set; } = new();
        
        public int MinPlayers { get; set; } = 0;
        public int MaxPlayers { get; set; } = 0;

        public Role NewRoleBoxRole { get; set; } = new Role();        
        public Role SelectedRole { get; set; } = new Role();      
        
        public  bool IsRuleOpen = false;
        public Rule newRule { get; set; } = new Rule();
        public List<Rule> Rules { get; set; } = new();
        public List<Rule> RuleTemplates = new List<Rule>();

        public async Task SetEventDateTime(string dateTimeString, Action<DateTime> setDateTimeAction)
        {
            var dateTimeFormat = "yyyy-MM-dd HH:mm";

            if (DateTime.TryParseExact(dateTimeString, dateTimeFormat, null, System.Globalization.DateTimeStyles.AssumeUniversal, out var parsedDateTime))
            {
                setDateTimeAction(parsedDateTime);
            }
            else
            {
                Console.WriteLine($"Invalid date-time format: {dateTimeString}");
            }
        }

        public void OnCommunitySelected(string id)
        {
            if (ObjectId.TryParse(id, out var objectId))
            {
                var selected = Communities.FirstOrDefault(s => s._id == objectId);
                if (selected != null)
                {
                    CommunityInput = new Community
                    {
                        _id = selected._id,
                        Name = selected.Name,
                    };
                }
                else
                {
                    CommunityInput = new Community();
                }
            }
        }


        public void OnGameSelected(string id)
        {
            if (ObjectId.TryParse(id, out var objectId))
            {
                var selected = Games.FirstOrDefault(s => s._id == objectId);
                if (selected != null)
                {
                    GameInput = new Game
                    {
                        _id = selected._id,
                        Name = selected.Name,
                        TitleImageUrl = selected.TitleImageUrl
                    };
                }
                else
                {
                    GameInput = new Game(); // No selection made
                }
            }
        }


        public void OnGameServerSelected(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                GameServerInput = new GameServer(); // New blank
            }
            else if (ObjectId.TryParse(id, out var objectId))
            {
                var selected = GameServers.FirstOrDefault(s => s._id == objectId);
                if (selected != null)
                {
                    GameServerInput = new GameServer
                    {
                        _id = ObjectId.GenerateNewId(),
                        Name = selected.Name,
                        IP = selected.IP,
                        Port = selected.Port,
                        Password = selected.Password
                    };
                }
            }
        }


        public void OnTSServerSelected(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TSServerInput = new TSServer(); // New blank
            }
            else if (ObjectId.TryParse(id, out var objectId))
            {
                var selected = TSServers.FirstOrDefault(s => s._id == objectId);
                if (selected != null)
                {
                    TSServerInput = new TSServer
                    {
                        _id = ObjectId.GenerateNewId(),
                        Name = selected.Name,
                        IP = selected.IP,
                        Port = selected.Port,
                        Password = selected.Password
                    };
                }
            }
        }

        public void AddRoleToRoleBox()
        {
            if (string.IsNullOrEmpty(NewRoleBoxRole.IdStr) || string.IsNullOrWhiteSpace(NewRoleBoxRole.Name)) return;

            var newRole = new Role
            {
                _id = ObjectId.GenerateNewId(),
                Name = NewRoleBoxRole.Name,
                RadioCall = NewRoleBoxRole.RadioCall,
                Order = NewRoleBoxRole.Order,
                IsLeaderSlot = NewRoleBoxRole.IsLeaderSlot
            };
            NewRoleBox.Roles.Add(newRole);
            ChangeRoleOrder(newRole);
            NewRoleBoxRole = new Role();
            NewRoleBoxRole.Order = NewRoleBox.Roles.Count + 1;
        }

        public void RemoveRoleFromNewRoleBox(string idStr)
        {
            var roleToRemove = NewRoleBox.Roles.FirstOrDefault(r => r.IdStr == idStr);
            if (roleToRemove != null)
            {
                NewRoleBox.Roles.Remove(roleToRemove);
            }
        }

        public void OpenRoleBoxModal()
        {
            NewRoleBox = new();
            IsRoleBoxModalOpen = true;
        }
        public void ShowBoxData(RoleBox rolebox)
        {
            NewRoleBox = rolebox;
             IsRoleBoxModalOpen = true;
        }
        public void CloseRoleBoxModal() => IsRoleBoxModalOpen = false;
        public void DeleteRoleBox(ObjectId boxId)
        {
            var box = RoleBoxes.FirstOrDefault(b => b._id == boxId);
            if (box != null)
            {
                RoleBoxes.Remove(box);
            }
        }

        public void CloseModal()
        {
            ShowDataBox = false;
            SelectedBox = null;
        }


        public void ChangeRoleOrder(Role changedRole)
        {
            var roles = NewRoleBox.Roles.OrderBy(r => r.Order).ToList();
            roles.RemoveAll(r => r._id == changedRole._id);
            int newOrder = Math.Max(1, Math.Min(changedRole.Order, roles.Count + 1));
            roles.Insert(newOrder - 1, changedRole);
            for (int i = 0; i < roles.Count; i++)
            {
                roles[i].Order = i + 1;
            }

            NewRoleBox.Roles = roles;
        }



        public void StartEditingRole (Role role)
        {
            Console.WriteLine("Editing Role -TODO");
        }


        public void AddRule()
        {
            if (string.IsNullOrEmpty(newRule.IdStr)) return;
            {
                Rules.Add(newRule);
                newRule = new Rule();
            }
        }

        public void RemoveRule(string idStr)
        {
            var ruleToRemove = Rules.FirstOrDefault(r => r.IdStr == idStr);
            if (ruleToRemove != null)
            {
                Rules.Remove(ruleToRemove);
            }
        }
        public void OpenRuleModal()
        {
            IsRuleOpen = true;
            newRule = new Rule();
        }
        public void CloseRuleModal() => IsRuleOpen = false; 

        public void ModifyRule(string idStr)
        {
            var ruleToModify = Rules.FirstOrDefault(r => r.IdStr == idStr);
            if (ruleToModify != null)
            {
                newRule = ruleToModify;
                IsRuleOpen = true;
            }
        }

        public DateTime ToUtc(DateTime local)
        {
            return DateTime.SpecifyKind(local, DateTimeKind.Local).ToUniversalTime();
        }
    }
}