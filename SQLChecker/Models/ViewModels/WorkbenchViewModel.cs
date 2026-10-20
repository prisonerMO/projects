using MongoDB.Bson.Serialization.Attributes;
using SQLChecker.Models;

namespace SQLChecker.ViewModels
{
    public class WorkbenchViewModel
    {
        public User User { get; set; }
        public Course Course { get; set; } = null!;
        public List<Assignment> Assignments { get; set; } = new();
        public Dictionary<string, List<string>> DbSchema { get; set; } = new();
        public string? SelectedDatabase { get; set; }
    }
}