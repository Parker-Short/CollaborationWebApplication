using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborationWebApplication.Pages.FileUploads
{
    public class FileHandlingModel : PageModel
    {
        private readonly string _connectionString;

        public FileHandlingModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CollabAppString");
        }

        public async Task<IActionResult> OnGetAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                ModelState.AddModelError("", "File path is not provided.");
                return Page();
            }

            if (!System.IO.File.Exists(filePath))
            {
                ModelState.AddModelError("", "File does not exist.");
                return Page();
            }

            try
            {
                await ProcessCsvFile(filePath);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error processing file: {ex.Message}");
                return Page();
            }

            return RedirectToPage("/Datasets/DatasetForm");
        }

        private async Task ProcessCsvFile(string filePath)
        {
            string tableName = Path.GetFileNameWithoutExtension(filePath);

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Configuration settings go here if needed
                // HasHeaderRecord = true is default and may not need to be set explicitly
            }))
            {
                csv.Read();
                csv.ReadHeader();
                var headers = csv.Context.Reader.HeaderRecord; // Correct way to access headers

                var records = new List<dynamic>();
                while (csv.Read())
                {
                    var record = csv.GetRecord<dynamic>();
                    records.Add(record);
                }

                await CreateTableFromCsv(tableName, headers, records);
            }
        }

        private async Task CreateTableFromCsv(string tableName, string[] headers, IEnumerable<dynamic> records)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var createTableSql = GenerateTableCreationSql(tableName, headers);
                using (var command = new SqlCommand(createTableSql, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
                await InsertDataIntoTable(tableName, headers, records, connection);
            }
        }

        private string GenerateTableCreationSql(string tableName, string[] headers)
        {
            var columnDefinitions = headers.Select(header =>
                $"[{header.Replace(" ", "_").Replace("-", "_")}] VARCHAR(255)");
            string sql = $@"CREATE TABLE [{tableName}] ({string.Join(", ", columnDefinitions)});";
            return sql;
        }

        private async Task InsertDataIntoTable(string tableName, string[] headers, IEnumerable<dynamic> records, SqlConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                foreach (var record in records)
                {
                    var columnList = string.Join(", ", headers.Select(header => $"[{header}]"));
                    var valueList = string.Join(", ", headers.Select(header =>
                    {
                        var value = record[header];
                        if (value == null) return "NULL";

                        return value is string ? $"'{value.ToString().Replace("'", "''")}'" :
                               value is DateTime ? $"'{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")}'" :
                               value.ToString();
                    }));

                    var insertSql = $"INSERT INTO [{tableName}] ({columnList}) VALUES ({valueList});";

                    using (var command = new SqlCommand(insertSql, connection, transaction))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }

                transaction.Commit();
            }
        }
    }
}
