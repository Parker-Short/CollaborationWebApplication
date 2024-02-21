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


        // SINGLE USER READER --- NOT CURRENTLY IN USE
        public static SqlDataReader SingleUserReader(int UserID)
        {
            var connection = new SqlConnection(CollabAppString);
            var cmdUserRead = new SqlCommand($"SELECT * FROM UserData WHERE UserID = {UserID}", connection);
            connection.Open();
            return cmdUserRead.ExecuteReader(CommandBehavior.CloseConnection);
        }

        // UPDATE USER --- NOT CURRENTLY IN USE
        public static void UpdateUser(User p)
        {
            using (var connection = new SqlConnection(CollabAppString))
            {
                var sqlQuery = $@"
                    UPDATE UserData SET 
                        FirstName = '{p.FirstName.Replace("'", "''")}',
                        LastName = '{p.LastName.Replace("'", "''")}',
                        Email = '{p.Email.Replace("'", "''")}',
                        Phone = '{p.Phone.Replace("'", "''")}',
                        Address = '{p.Address.Replace("'", "''")}' 
                    WHERE UserID = {p.UserID}";

                using (var cmdUserRead = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    cmdUserRead.ExecuteNonQuery();
                }
            }
        }

        // INSERT USER DATA -- CHANGE TO GENERAL INSERT
        public static void InsertUserData(User p)
        {
            using (var connection = new SqlConnection(CollabAppString))
            {
                var sqlQuery = $@"
                    INSERT INTO UserData (FirstName, LastName, Email, Phone, Address) VALUES (
                        '{p.FirstName.Replace("'", "''")}',
                        '{p.LastName.Replace("'", "''")}',
                        '{p.Email.Replace("'", "''")}',
                        '{p.Phone.Replace("'", "''")}',
                        '{p.Address.Replace("'", "''")}')";

                using (var cmdUserRead = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    cmdUserRead.ExecuteNonQuery();
                }
            }
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


        // SINGLE PLAN READER
        public static SqlDataReader SinglePlanReader(int PlanID)
        {
            var connection = new SqlConnection(CollabAppString);
            var cmdUserRead = new SqlCommand($"SELECT * FROM PlanData WHERE PlanID = {PlanID}", connection);
            connection.Open();
            return cmdUserRead.ExecuteReader(CommandBehavior.CloseConnection);
        }

        //UPDATE PLAN
        public static void UpdatePlan(Plan p)
        {
            String sqlQuery = "Update PlanData SET ";


        }

        // New Methods for DBClass.cs
        // Clear-Text Login
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



        //public static bool HashedParameterLogin(string Username, string Password)
        //{
        //    string loginQuery =
        //        "SELECT Password FROM HashedCredentials WHERE Username = @Username";

        //    SqlCommand cmdLogin = new SqlCommand();
        //    cmdLogin.Connection = CollabAppConnection;
        //    cmdLogin.Connection.ConnectionString = CollabAppString;

        //    cmdLogin.CommandText = loginQuery;
        //    cmdLogin.Parameters.AddWithValue("@Username", Username);

        //    cmdLogin.Connection.Open();

        //    // ExecuteScalar() returns back data type Object
        //    // Use a typecast to convert this to an int.
        //    // Method returns first column of first row.
        //    SqlDataReader hashReader = cmdLogin.ExecuteReader();
        //    if (hashReader.Read())
        //    {
        //        string correctHash = hashReader["Password"].ToString();

        //        if (PasswordHash.ValidatePassword(Password, correctHash))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}




        //public static void CreateHashedUser(string Username, string Password)
        //{
        //    string loginQuery =
        //        "INSERT INTO HashedCredentials (Username,Password) values (@Username, @Password)";

        //    SqlCommand cmdLogin = new SqlCommand();
        //    cmdLogin.Connection = CollabAppConnection;
        //    cmdLogin.Connection.ConnectionString = CollabAppString;

        //    cmdLogin.CommandText = loginQuery;
        //    cmdLogin.Parameters.AddWithValue("@Username", Username);
        //    cmdLogin.Parameters.AddWithValue("@Password", PasswordHash.HashPassword(Password));

        //    cmdLogin.Connection.Open();

        //    // ExecuteScalar() returns back data type Object
        //    // Use a typecast to convert this to an int.
        //    // Method returns first column of first row.
        //    cmdLogin.ExecuteNonQuery();

        //}
    }
}
