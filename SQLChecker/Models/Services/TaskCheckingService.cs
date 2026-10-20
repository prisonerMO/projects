using System.Text.RegularExpressions;
using SQLChecker.Services;

namespace SQLChecker.Models.Services
{
    public class CheckResult
    {
        public string Type { get; set; }
        public bool Passed { get; set; }
        public string Message { get; set; }
    }

    public class TaskCheckingService : ITaskCheckingService
    {
        private readonly IDatabaseService _sql;

        public TaskCheckingService(IConfiguration configuration, IDatabaseService databaseService)
        {
            _sql = databaseService;
        }

        public TaskCheckingService(IDatabaseService sql)
        {
            _sql = sql;
        }

        public async Task<(bool Passed, string Error)> RunTask(TaskDefinition task, string studentSql)
        {
            try
            {
                var expected = await _sql.ExecuteAsync(task.ExpectedSql);
                var student = await _sql.ExecuteAsync(studentSql);

                foreach (var check in task.Checks)
                {
                    bool ok = RunCheck(check, studentSql, expected, student);
                    if (!ok)
                        return (false, BuildFailMessage(check));
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<List<CheckResult>> RunAllChecks(TaskDefinition task, string studentSql)
        {
            var results = new List<CheckResult>();

            try
            {
                var expected = await _sql.ExecuteAsync(task.ExpectedSql);
                var student = await _sql.ExecuteAsync(studentSql);

                foreach (var check in task.Checks)
                {
                    bool passed;
                    string message;

                    try
                    {
                        passed = RunCheck(check, studentSql, expected, student);
                        message = passed ? "OK" : BuildFailMessage(check);
                    }
                    catch (Exception ex)
                    {
                        passed = false;
                        message = $"Error in {check.Type}: {ex.Message}";
                    }

                    results.Add(new CheckResult
                    {
                        Type = check.Type,
                        Passed = passed,
                        Message = message
                    });
                }
            }
            catch (Exception ex)
            {
                results.Add(new CheckResult
                {
                    Type = "SQL Execution",
                    Passed = false,
                    Message = $"Could not execute SQL: {ex.Message}"
                });
            }

            return results;
        }

        private bool RunCheck(
            Check check,
            string studentSql,
            List<List<string>> expected,
            List<List<string>> student)
        {
            string sql = studentSql.Trim();

            switch (check.Type)
            {
                case "Contains":
                    return sql.Contains(check.Value, StringComparison.OrdinalIgnoreCase);

                case "NotContains":
                    return !sql.Contains(check.Value, StringComparison.OrdinalIgnoreCase);

                case "StartsWithSelect":
                    return sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase);

                case "HasWhereClause":
                    return Regex.IsMatch(sql, @"\bWHERE\b", RegexOptions.IgnoreCase);

                case "HasOrderBy":
                case "OrderBy":
                    return Regex.IsMatch(sql, @"\bORDER\s+BY\b", RegexOptions.IgnoreCase);

                case "HasGroupBy":
                    return Regex.IsMatch(sql, @"\bGROUP\s+BY\b", RegexOptions.IgnoreCase);

                case "HasJoin":
                    return Regex.IsMatch(sql, @"\bJOIN\b", RegexOptions.IgnoreCase);

                case "Limit":
                    return Regex.IsMatch(sql,
                        $@"\bLIMIT\s+{Regex.Escape(check.Value)}\b",
                        RegexOptions.IgnoreCase);

                case "NoDuplicates":
                    var dataRows = student.Skip(1).Select(r => string.Join("|", r)).ToList();
                    return dataRows.Distinct().Count() == dataRows.Count;

                case "Custom":
                    return Regex.IsMatch(sql, check.Value, RegexOptions.IgnoreCase);

                case "CompareResult":
                    return Compare(expected, student);

                case "RowCount":
                    if (!int.TryParse(check.Value, out int expectedRows))
                        throw new ArgumentException($"RowCount needs an integer, got '{check.Value}'");
                    return (student.Count - 1) == expectedRows;

                case "FieldCount":
                    if (!int.TryParse(check.Value, out int expectedCols))
                        throw new ArgumentException($"FieldCount needs an integer, got '{check.Value}'");
                    return student.Count > 0 && student[0].Count == expectedCols;

                case "ColumnExists":
                    return student.Count > 0 &&
                           student[0].Any(col => col.Equals(check.Value, StringComparison.OrdinalIgnoreCase));

                default:
                    Console.WriteLine($"[TaskCheckingService] Unknown check type: '{check.Type}'");
                    return false;
            }
        }
        private bool Compare(List<List<string>> a, List<List<string>> b)
        {
            if (a.Count != b.Count) return false;

            for (int i = 0; i < a.Count; i++)
                if (!a[i].SequenceEqual(b[i]))
                    return false;

            return true;
        }

        private string BuildFailMessage(Check check) => check.Type switch
        {
            "Contains" => $"Query must contain '{check.Value}'",
            "NotContains" => $"Query must not contain '{check.Value}'",
            "StartsWithSelect" => "Query must start with SELECT",
            "HasWhereClause" => "Query must include a WHERE clause",
            "HasOrderBy"
            or "OrderBy" => "Query must include ORDER BY",
            "HasGroupBy" => "Query must include GROUP BY",
            "HasJoin" => "Query must include a JOIN",
            "Limit" => $"Query must include LIMIT {check.Value}",
            "NoDuplicates" => "Result set must not contain duplicate rows",
            "Custom" => $"Query must match pattern: {check.Value}",
            "CompareResult" => "Result does not match the expected output",
            "RowCount" => $"Result must contain exactly {check.Value} data rows",
            "FieldCount" => $"Result must have exactly {check.Value} columns",
            "ColumnExists" => $"Result must contain a column named '{check.Value}'",
            _ => $"Check '{check.Type}' failed"
        };
    }
}
