using CollaborationWebApplication.Pages.DB;
using CollaborationWebApplication.Pages.DataClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CollaborationWebApplication.Pages.Collaboration
{

    

    public class CollabDashboardModel : PageModel
    {
        [BindProperty]
        public int CollabID { get; set; }
        [BindProperty]

 

        public List<SelectListItem> AvailableUsers { get; set; }
        public List<SelectListItem> AvailableKnowledgeItems { get; set; }
        public List<SelectListItem> AvailablePlans { get; set; }
        public List<SelectListItem> AvailableSwots { get; set; }
        public List<SelectListItem> CollaborationUsers { get; set; }

        [BindProperty]
        public DataClasses.Chat NewChat { get; set; }


        public IActionResult OnGetSessionCheck() // Check if user is logged in
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login/HashedLogin");
            }
            else
            {
                return Page();
            }
        }

        public void OnGet(int collabID, string collabName)
        { // Fetch necessary data when page loads

            CollabID = collabID;

            AvailableUsers = FetchAvailableUsers();
            AvailableKnowledgeItems = FetchAvailableKnowledgeItems();
            AvailablePlans = FetchAvailablePlans();
            AvailableSwots = FetchAvailableSwots();
            CollaborationUsers = FetchCollaborationUsers();
        }


        // Methods for fetching data
        private List<SelectListItem> FetchCollaborationUsers()
        {
            CollaborationUsers = new List<SelectListItem>();
            string sqlQuery = $@"
                SELECT UserData.UserID, (UserData.FirstName + ' ' + UserData.LastName) AS FullName 
                FROM UserData 
                JOIN CollabUser ON UserData.UserID = CollabUser.UserID 
                WHERE CollabUser.CollabID = {CollabID};
            ";

            using (var reader = DBClass.GeneralReaderQuery(sqlQuery)) // Fetch users who are part of the collaboration
            // Populate CollaborationUsers list
            {
                while (reader.Read())
                {
                    CollaborationUsers.Add(new SelectListItem
                    {
                        Value = reader["UserID"].ToString(),
                        Text = reader["FullName"].ToString()
                    });
                }
            }
            return CollaborationUsers;
        }

        private List<SelectListItem> FetchAvailableUsers() // Fetch users who are not part of the collaboration
            // Populate AvailableUsers list
        {
            var users = new List<SelectListItem>();
            string sqlQuery = @"
                SELECT UserID, FirstName + ' ' + LastName AS FullName
                FROM UserData
                WHERE UserID NOT IN (
                    SELECT UserID
                    FROM CollabUser
                    WHERE CollabID = @CollabID
                );";

            using (var reader = DBClass.GeneralReaderQuery(sqlQuery.Replace("@CollabID", CollabID.ToString())))
            {
                while (reader.Read())
                {
                    users.Add(new SelectListItem
                    {
                        Value = reader["UserID"].ToString(),
                        Text = reader["FullName"].ToString()
                    });
                }
            }
            return users;
        }

        private List<SelectListItem> FetchAvailableKnowledgeItems()
        {
            var items = new List<SelectListItem>();
            string sqlQuery = $@"
                SELECT KnowledgeItemID, KnowledgeTitle
                FROM KnowledgeItem
                WHERE KnowledgeItemID NOT IN (
                    SELECT KnowledgeItemID
                    FROM CollabKnowledge
                    WHERE CollabID = {CollabID}
                );";

            using (var reader = DBClass.GeneralReaderQuery(sqlQuery))
            {
                while (reader.Read())
                {
                    items.Add(new SelectListItem
                    {
                        Value = reader["KnowledgeItemID"].ToString(),
                        Text = reader["KnowledgeTitle"].ToString()
                    });
                }
            }
            return items;
        }
         
        private List<SelectListItem> FetchAvailablePlans()
        {
            var plans = new List<SelectListItem>();
            string sqlQuery = $@"
                SELECT PlanID, PlanName
                FROM PlanData
                WHERE PlanID NOT IN (
                    SELECT PlanID
                    FROM CollabPlan
                    WHERE CollabID = {CollabID}
                );";

            using (var reader = DBClass.GeneralReaderQuery(sqlQuery))
            {
                while (reader.Read())
                {
                    plans.Add(new SelectListItem
                    {
                        Value = reader["PlanID"].ToString(),
                        Text = reader["PlanName"].ToString()
                    });
                }
            }
            return plans;
        }
        
        private List<SelectListItem> FetchAvailableSwots()
        {
            List<SelectListItem> swots = new List<SelectListItem>();
            string sqlQuery = "SELECT SwotID, (Strength + ', ' + Weakness + ', ' + Opportunity + ', ' + Threat) AS SwotSummary FROM Swot WHERE SwotID NOT IN (SELECT SwotID FROM CollabSwot WHERE CollabID = @CollabID);";
            using (var reader = DBClass.GeneralReaderQuery(sqlQuery.Replace("@CollabID", CollabID.ToString())))
            {
                while (reader.Read())
                {
                    swots.Add(new SelectListItem { Value = reader["SwotID"].ToString(), Text = reader["SwotSummary"].ToString() });
                }
            }
            return swots;
        }
        // method to add users to the collaboration
        public IActionResult OnPostAddUser(int UserID)
        {
            string query = $"INSERT INTO CollabUser (UserID, CollabID) VALUES ({UserID}, {CollabID})";
            DBClass.InsertQuery(query);
            return RedirectToPage("./CollabDashboard", new { collabID = CollabID});
        }
        // method to add knowledge items to the collaboration 
        public IActionResult OnPostAddKnowledgeItem(int KnowledgeItemID)
        {
            string query = $"INSERT INTO CollabKnowledge (KnowledgeItemID, CollabID) VALUES ({KnowledgeItemID}, {CollabID})";
            DBClass.InsertQuery(query);
            return RedirectToPage("./CollabDashboard", new { collabID = CollabID});
        }
        // method to add plan to the collaboration
        public IActionResult OnPostAddPlan(int PlanID)
        {
            string query = $"INSERT INTO CollabPlan (PlanID, CollabID) VALUES ({PlanID}, {CollabID})";
            DBClass.InsertQuery(query);
            return RedirectToPage("./CollabDashboard", new { collabID = CollabID});
        }
        // Method to handle adding a SWOT analysis to the collaboration
        public IActionResult OnPostAddSwot(int SwotID)
        { // Construct the SQL query to insert the SWOT analysis into the collaboration
            string query = $"INSERT INTO CollabSwot (SwotID, CollabID) VALUES ({SwotID}, {CollabID})";
            DBClass.InsertQuery(query);
            return RedirectToPage("./CollabDashboard", new { collabID = CollabID});
        }



        public IActionResult OnPostAddChat()
        {
            // Retrieve Username from session
            string username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username))
            {
                // Handle the case where the username is not in the session (user not logged in)
                return RedirectToPage("/Login/HashedLogin");
            }

            // Fetch UserID based on Username
            int userID = DBClass.FetchUserIDForUsername(username);
            if (userID == -1)
            {
                // Handle the case where UserID couldn't be fetched
                // This might involve logging the error and redirecting to an error page or login page
                return RedirectToPage("/Login/HashedLogin");
            }

            // Construct and execute the SQL command to insert the new chat message
            string sqlQuery = "INSERT INTO Chat (Content, UserID, CollabID) VALUES (@Content, @UserID, @CollabID)";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@Content", NewChat.Content},
                    {"@UserID", userID}, // Use the fetched UserID for the chat message
                    {"@CollabID", CollabID }
                };

            DBClass.ExecuteSqlCommand(sqlQuery, parameters);

            // Redirect back to the dashboard or wherever appropriate
            return RedirectToPage("./CollabDashboard", new { collabID = CollabID });
        }


    }
}