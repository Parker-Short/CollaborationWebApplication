using CollaborationWebApplication.Pages.DataClasses;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Data.SqlClient;

namespace CollaborationWebApplication.Pages.DB
{
    public class DBClass
    {

        private static readonly String? AuthConnString = "Server=Localhost;Database=AUTH;Trusted_Connection=True";

        

        public static SqlConnection CollabAppConnection = new SqlConnection();

        // DB Connection String
        private static readonly string CollabAppString = "server=Localhost;Database=Lab3;Trusted_Connection=True";

        //METHOD TO TEST GENERAL INSERT + PARAMETERS


        public static int FetchUserIDForUsername(string username)
        {
            string sqlQuery = "SELECT UserID FROM UserData WHERE Username = @Username"; // Adjust table/column names as necessary
            using (var connection = new SqlConnection(CollabAppString)) // Ensure AuthConnString is correctly defined
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                connection.Open();
                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1; // Return -1 or handle appropriately if user not found
            }
        }


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

        //New code for CreateHashUser
        public static void CreateHashedUser(string Username, string Password, string FirstName, string LastName, string Email, string Phone, string Address)
        {
            string loginQuery =
                "INSERT INTO HashedCredentials (Username,Password) values (@Username, @Password)";
            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = CollabAppConnection;
            cmdLogin.Connection.ConnectionString = AuthConnString;

            cmdLogin.CommandText = loginQuery;
            cmdLogin.Parameters.AddWithValue("@Username", Username);
            cmdLogin.Parameters.AddWithValue("@Password", PasswordHash.HashPassword(Password));

            cmdLogin.Connection.Open();
            //user typecast to return this to an int
            //Method returns first column of first row
            cmdLogin.ExecuteNonQuery();

            // Insert into UserDetails table in the lab3 database
            string userDetailsQuery = "INSERT INTO UserData (Username, FirstName, LastName, Email, Phone, Address) VALUES (@Username, @FirstName, @LastName, @Email, @Phone, @Address)";
            using (SqlConnection collabConn = new SqlConnection(CollabAppString))
            {
                SqlCommand cmdUserDetails = new SqlCommand(userDetailsQuery, collabConn);
                cmdUserDetails.Parameters.AddWithValue("@Username", Username);
                cmdUserDetails.Parameters.AddWithValue("@FirstName", FirstName);
                cmdUserDetails.Parameters.AddWithValue("@LastName", LastName);
                cmdUserDetails.Parameters.AddWithValue("@Email", Email);
                cmdUserDetails.Parameters.AddWithValue("@Phone", Phone);
                cmdUserDetails.Parameters.AddWithValue("@Address", Address);
                collabConn.Open();
                cmdUserDetails.ExecuteNonQuery();
                collabConn.Close();
                
            }
            cmdLogin.Connection.Close();

        }

        public static bool HashedParameterLogin(string Username, string Password)
        {
            string loginQuery =
                "SELECT Password FROM HashedCredentials WHERE Username = @Username";

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = CollabAppConnection;
            
            cmdLogin.Connection.ConnectionString = AuthConnString;

            cmdLogin.CommandText = loginQuery;
            cmdLogin.Parameters.AddWithValue("@Username", Username);

            cmdLogin.Connection.Open();

            SqlDataReader hashReader = cmdLogin.ExecuteReader();
            if (hashReader.Read())
            {
                string correctHash = hashReader["Password"].ToString();

                if (PasswordHash.ValidatePassword(Password, correctHash))
                {
                    return true;
                    cmdLogin.Connection.Close();
                    
                }
            }
            return false;

            cmdLogin.Connection.Close();
        }


        // General Reader
        public static SqlDataReader GeneralReaderQueryAUTH(string sqlQuery)
        {
            var connection = new SqlConnection(AuthConnString);
            var cmdGeneralRead = new SqlCommand(sqlQuery, connection);
            connection.Open();
            return cmdGeneralRead.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }



    }
}
