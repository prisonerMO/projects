namespace SQLChecker.Models.ViewModels
{
    public class QueryResultViewModel
    {
        public List<string> Columns { get; set; } = new();
        public List<List<string>> Rows { get; set; } = new();
        public string? Error { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
        public bool HasRows => Rows.Any();
    }
}
