        //Check is user is logged in        // Fetch the available plans //SQL query to get plan        // Fetch the available swots//Sql query to get the SWOTusing Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using CollaborationWebApplication.Pages.DB;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CollaborationWebApplication.Pages.Collaboration
{
    public class PrintReportModel : PageModel
    {
        private static readonly string CollabAppString = "server=Localhost;Database=Lab3;Trusted_Connection=True";

        public int CollabID { get; set; }
        public string CollabName { get; set; }
        public DateTime ReportDate { get; set; } = DateTime.Now;
        public List<dynamic> Plans { get; set; }
        public List<dynamic> Swots { get; set; }
        public List<dynamic> KnowledgeItems { get; set; }
        public List<dynamic> ContributingUsers { get; set; }

        public void OnGet(int collabID)
        {
            CollabID = collabID;
            FetchCollaborationDetails(collabID);
        }

        private void FetchCollaborationDetails(int collabID)
        {
            // Initialize your lists
            Plans = new List<dynamic>();
            Swots = new List<dynamic>();
            KnowledgeItems = new List<dynamic>();
            ContributingUsers = new List<dynamic>();

            // Fetch Collaboration Name
            CollabName = FetchCollaborationName(collabID);

            // Fetch Plans associated with the Collaboration
            Plans = FetchPlans(collabID);

            // Fetch SWOTs associated with the Collaboration
            Swots = FetchSwots(collabID);

            // Fetch Knowledge Items associated with the Collaboration
            KnowledgeItems = FetchKnowledgeItems(collabID);

            // Fetch Contributing Users associated with the Collaboration
            ContributingUsers = FetchContributingUsers(collabID);
        }

        private string FetchCollaborationName(int collabID)
        {
            string collabName = "";
            string sqlQuery = "SELECT CollabName FROM Collaboration WHERE CollabID = @CollabID";
            using (SqlConnection conn = new SqlConnection(CollabAppString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@CollabID", collabID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            collabName = reader["CollabName"].ToString();
                        }
                    }
                }
            }
            return collabName;
        }


        private List<dynamic> FetchPlans(int collabID)
        {
            var plans = new List<dynamic>();
            string sqlQuery = @"
        SELECT pd.PlanID, pd.PlanName, pd.Content, ud.FirstName + ' ' + ud.LastName AS CreatedBy
        FROM PlanData pd
        INNER JOIN CollabPlan cp ON pd.PlanID = cp.PlanID
        INNER JOIN UserData ud ON pd.UserID = ud.UserID
        WHERE cp.CollabID = @CollabID";

            using (SqlConnection conn = new SqlConnection(CollabAppString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@CollabID", collabID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            plans.Add(new
                            {
                                PlanID = reader["PlanID"],
                                PlanName = reader["PlanName"].ToString(),
                                Content = reader["Content"].ToString(),
                                CreatedBy = reader["CreatedBy"].ToString() // Ensure this matches the SQL column alias
                            });
                        }
                    }
                }
            }
            return plans;
        }




        private List<dynamic> FetchSwots(int collabID)
        {
            var swots = new List<dynamic>();
            string sqlQuery = @"
        SELECT s.SwotID, s.SwotName, s.Strength, s.Weakness, s.Opportunity, s.Threat,
               (ud.FirstName + ' ' + ud.LastName) AS CreatedBy
        FROM Swot s
        INNER JOIN CollabSwot cs ON s.SwotID = cs.SwotID
        INNER JOIN UserData ud ON s.UserID = ud.UserID
        WHERE cs.CollabID = @CollabID";

            using (SqlConnection conn = new SqlConnection(CollabAppString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@CollabID", collabID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            swots.Add(new
                            {
                                SwotID = reader["SwotID"],
                                SwotName = reader["SwotName"].ToString(),
                                Strength = reader["Strength"].ToString(),
                                Weakness = reader["Weakness"].ToString(),
                                Opportunity = reader["Opportunity"].ToString(),
                                Threat = reader["Threat"].ToString(),
                                CreatedBy = reader["CreatedBy"].ToString() // Make sure you fetch this from the joined UserData
                            });
                        }
                    }
                }
            }
            return swots;
        }




        private List<dynamic> FetchKnowledgeItems(int collabID)
        {
            var knowledgeItems = new List<dynamic>();
            string sqlQuery = @"
        SELECT ki.KnowledgeItemID, ki.KnowledgeTitle, ki.KnowledgeInformation,
               (ud.FirstName + ' ' + ud.LastName) AS CreatedBy
        FROM KnowledgeItem ki
        INNER JOIN CollabKnowledge ck ON ki.KnowledgeItemID = ck.KnowledgeItemID
        INNER JOIN UserData ud ON ki.UserID = ud.UserID
        WHERE ck.CollabID = @CollabID";

            using (SqlConnection conn = new SqlConnection(CollabAppString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@CollabID", collabID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            knowledgeItems.Add(new
                            {
                                KnowledgeItemID = reader["KnowledgeItemID"],
                                KnowledgeTitle = reader["KnowledgeTitle"].ToString(),
                                KnowledgeInformation = reader["KnowledgeInformation"].ToString(),
                                CreatedBy = reader["CreatedBy"].ToString() // Ensure this matches the SQL column alias
                            });
                        }
                    }
                }
            }
            return knowledgeItems;
        }


        private List<dynamic> FetchContributingUsers(int collabID)
        {
            var contributingUsers = new List<dynamic>();
            string sqlQuery = "SELECT UserID, FirstName, LastName FROM UserData WHERE UserID IN (SELECT UserID FROM CollabUser WHERE CollabID = @CollabID)";
            using (SqlConnection conn = new SqlConnection(CollabAppString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@CollabID", collabID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contributingUsers.Add(new
                            {
                                UserID = reader["UserID"],
                                FullName = reader["FirstName"].ToString() + " " + reader["LastName"].ToString()
                            });
                        }
                    }
                }
            }
            return contributingUsers;
        }

    }
}
