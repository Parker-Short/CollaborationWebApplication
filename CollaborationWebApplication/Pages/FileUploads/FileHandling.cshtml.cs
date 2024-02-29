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
using System.Dynamic;

namespace CollaborationWebApplication.Pages.FileUploads
{
    public class FileHandlingModel : PageModel
    {
        private static readonly string CollabAppString = "server=Localhost;Database=Lab3;Trusted_Connection=True";

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

            // Redirect to a success page and optionally pass the table name for confirmation.
            TempData["TableName"] = Path.GetFileNameWithoutExtension(filePath);
            return RedirectToPage("/FileUploads/CSVUploadStatus");
        }

        private async Task ProcessCsvFile(string filePath)
        {
            string tableName = Path.GetFileNameWithoutExtension(filePath).Replace(" ", "_").Replace("-", "_");

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", HeaderValidated = null, MissingFieldFound = null }))
            {
                var records = new List<IDictionary<string, object>>();
                csv.Read();
                csv.ReadHeader();
                var headers = csv.Context.Reader.HeaderRecord;

                while (csv.Read())
                {
                    var record = new ExpandoObject() as IDictionary<string, object>;
                    foreach (string header in headers)
                    {
                        record[header] = csv.GetField(header);
                    }
                    records.Add(record);
                }

                await CreateTableFromCsv(tableName, headers, records);
                await AddFileToDataSet(tableName);
            }
        }

        private async Task CreateTableFromCsv(string tableName, string[] headers, IEnumerable<IDictionary<string, object>> records)
        {
            using (var connection = new SqlConnection(CollabAppString))
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
            var columnDefinitions = headers.Select(header => $"[{header}] NVARCHAR(MAX)");
            string sql = $@"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'{tableName}')
                            BEGIN
                                CREATE TABLE [{tableName}] ({string.Join(", ", columnDefinitions)});
                            END";
            return sql;
        }

        private async Task InsertDataIntoTable(string tableName, string[] headers, IEnumerable<IDictionary<string, object>> records, SqlConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                foreach (var record in records)
                {
                    var columns = string.Join(", ", record.Keys.Select(key => $"[{key}]"));
                    var values = string.Join(", ", record.Values.Select(value => value == null ? "NULL" : $"'{value.ToString().Replace("'", "''")}'"));
                    var insertSql = $"INSERT INTO [{tableName}] ({columns}) VALUES ({values});";

                    using (var command = new SqlCommand(insertSql, connection, transaction))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
                transaction.Commit();
            }
        }

        public static List<(int Id, string TableName)> FetchAllDatasets()
        {
            List<(int Id, string TableName)> datasets = new List<(int Id, string TableName)>();
            string sqlQuery = "SELECT Id, TableName FROM Dataset"; // Adjust if your table or column names differ
            using (var connection = new SqlConnection("CollabAppString"))
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0); // Assumes Id is the first column
                        string tableName = reader.GetString(1); // Assumes TableName is the second column
                        datasets.Add((id, tableName));
                    }
                }
            }
            return datasets;
        }

        private async Task AddFileToDataSet(string tableName)
        {
            // Assuming the DataSet table has a column named "TableName"
            string addFileQuery = @"INSERT INTO DataSet (FileName) VALUES (@FileName);";

            using (var connection = new SqlConnection(CollabAppString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(addFileQuery, connection))
                {
                    // Parameterization to prevent SQL injection
                    command.Parameters.AddWithValue("@FileName", tableName);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }



    }
}
