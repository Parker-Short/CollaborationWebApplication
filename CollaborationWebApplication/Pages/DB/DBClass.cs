﻿using CollaborationWebApplication.Pages.DataClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CollaborationWebApplication.Pages.DB
{
    public class DBClass
    {
        public static SqlConnection CollabAppConnection = new SqlConnection();

        //String For AUTH DB
        private static readonly String? AuthConnString = "Server=Localhost;Database=AUTH;Trusted_Connection=True";
        //String For LAB3 DB
        private static readonly string CollabAppString = "server=Localhost;Database=Lab3;Trusted_Connection=True";


        //"#########################################################################################################################################################################"
        //"#########################################################################################################################################################################"

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

        //"#########################################################################################################################################################################"
        //"#########################################################################################################################################################################"

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

        //"########################################################################################################################################################################"
        //"#########################################################################################################################################################################"

        public static SqlDataReader GeneralReaderQuery(string sqlQuery)
        {
            var connection = new SqlConnection(CollabAppString);
            var cmdGeneralRead = new SqlCommand(sqlQuery, connection);
            connection.Open();
            return cmdGeneralRead.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        //"#########################################################################################################################################################################"
        //"#########################################################################################################################################################################"

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

        //"#########################################################################################################################################################################"
        //"#########################################################################################################################################################################"

        // USER DATA READER
        public static SqlDataReader UserReader()
        {
            var connection = new SqlConnection(CollabAppString);
            var cmdUserRead = new SqlCommand("SELECT * FROM USERDATA", connection);
            connection.Open();
            return cmdUserRead.ExecuteReader(CommandBehavior.CloseConnection);
        }

        //"#########################################################################################################################################################################"
        //"#########################################################################################################################################################################"
        //
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

        //"#########################################################################################################################################################################"
        //"#########################################################################################################################################################################"

        public static bool HashedParameterLogin(string Username, string Password)
        {
            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = CollabAppConnection;

            cmdLogin.Connection.ConnectionString = AuthConnString;
            cmdLogin.CommandType = System.Data.CommandType.StoredProcedure;
            /*cmdLogin.CommandText = loginQuery;*/
            cmdLogin.Parameters.AddWithValue("@Username", Username);
            cmdLogin.CommandText = "sp_HashedLogin";
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
        //"#########################################################################################################################################################################"
        //"#########################################################################################################################################################################"

        public static DataTable FetchDataForTable(string tableName)
        {
            // WARNING: Directly using user input in SQL queries can lead to SQL injection.
            // Ensure tableName is validated against a list of known, safe table names or use another form of verification.
            DataTable dataTable = new DataTable();
            string query = $"SELECT * FROM [{tableName}]"; // Unsafe: Do not use as is.

            using (var connection = new SqlConnection(CollabAppString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            return dataTable;
        }

        //"#########################################################################################################################################################################
        //"#########################################################################################################################################################################"

        // General Reader for AUTH database
        public static SqlDataReader GeneralReaderQueryAUTH(string sqlQuery)
        {
            var connection = new SqlConnection(AuthConnString);
            var cmdGeneralRead = new SqlCommand(sqlQuery, connection);
            connection.Open();
            return cmdGeneralRead.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
    }
}
    