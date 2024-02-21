using CollaborationWebApplication.Pages.DataClasses;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Data.SqlClient;

namespace CollaborationWebApplication.Pages.DB
{
    public class DBClass
    {

        public static SqlConnection CollabAppConnection = new SqlConnection();

        // DB Connection String
        private static readonly string CollabAppString = "server=Localhost;Database=Lab2;Trusted_Connection=True";

        //METHOD TO TEST GENERAL INSERT + PARAMETERS

        public static void ExecuteSqlCommand(string sqlQuery, Dictionary<string, object> parameters)
        {
            using (var connection = new SqlConnection(CollabAppString))
            {
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }



        // General Reader
        public static SqlDataReader GeneralReaderQuery(string sqlQuery)
        {
            var connection = new SqlConnection(CollabAppString);
            var cmdGeneralRead = new SqlCommand(sqlQuery, connection);
            connection.Open();
            return cmdGeneralRead.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        // GENERAL INSERT STATEMENT
        public static void InsertQuery(string sqlQuery)
        {
            using (var connection = new SqlConnection(CollabAppString))
            {
                using (var cmdGeneralRead = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    cmdGeneralRead.ExecuteNonQuery();
                }
            }
        }

        // USER DATA READER
        public static SqlDataReader UserReader()
        {
            var connection = new SqlConnection(CollabAppString);
            var cmdUserRead = new SqlCommand("SELECT * FROM USERDATA", connection);
            connection.Open();
            return cmdUserRead.ExecuteReader(CommandBehavior.CloseConnection);
        }

        // INSERT KNOWLEDGE ITEM CATEGORY DATA -- CHANGE TO GENERAL INSERT
        public static void InsertKnowledgeCategory(KnowledgeItemCategory k)
        {
            using (var connection = new SqlConnection(CollabAppString))
            {
                var sqlQuery = $@"
                    INSERT INTO KnowledgeItemCategory (CategoryName) VALUES (
                        '{k.CategoryName.Replace("'", "''")}')";

                using (var cmdKnowledgeCategoryRead = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    cmdKnowledgeCategoryRead.ExecuteNonQuery();
                }

            }
        }

        // Parameterized Login
        // Creating a User and Login with Password Hashing

        public static int LoginQuery(string loginQuery)
        {
            // This method expects to receive an SQL SELECT
            // query that uses the COUNT command.

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = CollabAppConnection;
            cmdLogin.Connection.ConnectionString = CollabAppString;
            cmdLogin.CommandText = loginQuery;
            cmdLogin.Connection.Open();

            // ExecuteScalar() returns back data type Object
            // Use a typecast to convert this to an int.
            // Method returns first column of first row.
            int rowCount = (int)cmdLogin.ExecuteScalar();

            return rowCount;
        }


        public static int SecureLogin(string Username, string Password)
        {
            string loginQuery =
                "SELECT COUNT(*) FROM Credentials where Username = @Username and Password = @Password";

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = CollabAppConnection;
            cmdLogin.Connection.ConnectionString = CollabAppString;

            cmdLogin.CommandText = loginQuery;
            cmdLogin.Parameters.AddWithValue("@Username", Username);
            cmdLogin.Parameters.AddWithValue("@Password", Password);

            cmdLogin.Connection.Open();

            // ExecuteScalar() returns back data type Object
            // Use a typecast to convert this to an int.
            // Method returns first column of first row.
            int rowCount = (int)cmdLogin.ExecuteScalar();

            return rowCount;
        }

    }
}
