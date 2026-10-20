using MongoDB.Driver.Core.Configuration;
using MySql.Data.MySqlClient;
using SQLChecker.Models;
using SQLChecker.Models.Services;

namespace SQLChecker.Services
{

    public class DatabaseService : IDatabaseService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public DatabaseService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public async Task<Database?> CreateDatabaseAsync(Database database)
        {
            try
            {
                await CreateMySqlDatabaseAsync(database.Name);

                var newDatabase = new Database
                {
                    Name = database.Name,
                    CreatedAt = database.CreatedAt
                };

                newDatabase.Save();
                return newDatabase;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateDatabaseAsync failed: {ex.Message}");
                throw;
            }
        }

        private async Task CreateMySqlDatabaseAsync(string dbName)
        {
            var sqlFilesPath = Path.Combine(_env.ContentRootPath, "SQLFiles");
            var schemaFile = Path.Combine(sqlFilesPath, "sakila-schema.sql");
            var dataFile = Path.Combine(sqlFilesPath, "sakila-data.sql");

            if (!File.Exists(schemaFile)) throw new Exception("sakila-schema.sql not found");
            if (!File.Exists(dataFile)) throw new Exception("sakila-data.sql not found");

            var baseCs = _configuration.GetConnectionString("SqlDatabaseConnection")!;
            var csBuilder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(baseCs)
            {
                Database = ""
            };

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(csBuilder.ToString()))
            {
                await conn.OpenAsync();
                using var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                    $"CREATE DATABASE IF NOT EXISTS `{dbName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;", conn);
                await cmd.ExecuteNonQueryAsync();
            }

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(csBuilder.ToString()))
            {
                await conn.OpenAsync();
                var schema = await File.ReadAllTextAsync(schemaFile);
                schema = schema.Replace("USE sakila;", $"USE `{dbName}`;", StringComparison.OrdinalIgnoreCase);
                schema = schema.Replace("sakila.", $"{dbName}.", StringComparison.OrdinalIgnoreCase);
                schema = schema.Replace("`sakila`.", $"`{dbName}`.", StringComparison.OrdinalIgnoreCase);
                await ExecuteSqlScriptAsync(conn, schema);
            }

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(csBuilder.ToString()))
            {
                await conn.OpenAsync();
                var data = await File.ReadAllTextAsync(dataFile);
                data = data.Replace("USE sakila;", $"USE `{dbName}`;", StringComparison.OrdinalIgnoreCase);
                data = data.Replace("sakila.", $"{dbName}.", StringComparison.OrdinalIgnoreCase);
                data = data.Replace("`sakila`.", $"`{dbName}`.", StringComparison.OrdinalIgnoreCase);
                await ExecuteSqlScriptAsync(conn, data);
            }


        }

        private async Task ExecuteSqlScriptAsync(MySql.Data.MySqlClient.MySqlConnection conn, string sql)
        {
            var statements = SplitSqlStatements(sql);
            foreach (var statement in statements)
            {
                var trimmed = statement.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                try
                {
                    using var cmd = new MySql.Data.MySqlClient.MySqlCommand(trimmed, conn);
                    cmd.CommandTimeout = 120;
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"FAILED:\n{trimmed}\n\nERROR:\n{ex.Message}");
                    throw;
                }
            }
        }
        private List<string> SplitSqlStatements(string sql)
        {
            var statements = new List<string>();
            var current = new System.Text.StringBuilder();
            string delimiter = ";";

            foreach (var line in sql.Split('\n'))
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("DELIMITER", StringComparison.OrdinalIgnoreCase))
                {
                    var curr = current.ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(curr))
                    {
                        statements.Add(curr);
                        current.Clear();
                    }
                    delimiter = trimmedLine.Substring("DELIMITER".Length).Trim();
                    continue;
                }
                current.AppendLine(line);
                if (trimmedLine.EndsWith(delimiter))
                {
                    var statement = current.ToString().Trim();
                    if (statement.EndsWith(delimiter))
                        statement = statement.Substring(0, statement.Length - delimiter.Length).Trim();
                    if (!string.IsNullOrWhiteSpace(statement))
                        statements.Add(statement);
                    current.Clear();
                }
            }
            var remaining = current.ToString().Trim();
            if (!string.IsNullOrWhiteSpace(remaining))
                statements.Add(remaining);

            return statements;
        }
        public async Task<Dictionary<string, List<string>>> GetSchemaAsync(string databaseName)
        {
            var schema = new Dictionary<string, List<string>>();
            using var conn = new MySqlConnection(_configuration.GetConnectionString("SqlDatabaseConnection")!);
            await conn.OpenAsync();

            var builder = new MySqlConnectionStringBuilder(conn.ConnectionString);
            var dbName = string.IsNullOrEmpty(databaseName) ? builder.Database : databaseName;

            // Get tables
            var tables = new List<string>();
            using (var cmd = new MySqlCommand(
                "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA = @db ORDER BY TABLE_NAME", conn))
            {
                cmd.Parameters.AddWithValue("@db", dbName);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    tables.Add(reader.GetString(0));
            }

            // Get columns per table
            foreach (var table in tables)
            {
                var cols = new List<string>();
                using var cmd = new MySqlCommand(
                    "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @table ORDER BY ORDINAL_POSITION", conn);
                cmd.Parameters.AddWithValue("@db", dbName);
                cmd.Parameters.AddWithValue("@table", table);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    cols.Add(reader.GetString(0));

                schema[table] = cols;
            }

            return schema;
        }
        private async Task ExecuteConnectionAsync(MySql.Data.MySqlClient.MySqlConnection conn, string sql)
        {
            using var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn);
            cmd.CommandTimeout = 120;
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<List<string>>> ExecuteAsync(string query)
        {
            if (!IsSafe(query))
                throw new Exception("Unsafe query detected!");

            var results = new List<List<string>>();

            using var conn = new MySqlConnection(_configuration.GetConnectionString("SqlDatabaseConnection")!);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var headers = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
                headers.Add(reader.GetName(i));
            results.Add(headers);

            while (await reader.ReadAsync())
            {
                var row = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row.Add(reader.GetValue(i)?.ToString() ?? "NULL");
                results.Add(row);
            }

            return results;
        }

        private bool IsSafe(string query)
        {
            var bad = new[] { "drop", "delete", "insert", "update" };
            string q = query.ToLower();

            return !bad.Any(b => q.Contains(b));
        }

        public async Task<Database?> GetDatabaseByIdAsync(string databaseId)
        {
            return await Task.Run(() => DB_Services.GetObjectByIdStr<Database>(databaseId));
        }
        public async Task<List<Database>> GetAllDatabasesAsync()
        {
            return await DB_Services.GetAllAsync<Database>();
        }
 
        public async Task UpdateDatabaseAsync(Database database )
        {
            await Task.Run(() => database.Save());
        }

        public async Task DeleteDatabaseAsync(string databaseId)
        {
            await Task.Run(() =>
            {
                var database = DB_Services.GetObjectByIdStr<Database>(databaseId);
                if (database != null)
                {
                    // Your delete logic
                }
            });
        }
    }

}
