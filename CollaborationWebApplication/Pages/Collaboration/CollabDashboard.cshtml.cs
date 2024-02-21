using CollaborationWebApplication.Pages.DB;
using CollaborationWebApplication.Pages.DataClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using CollaborationWebApplication.Pages.DB;

namespace CollaborationWebApplication.Pages.Collaboration
{

    //BEGIN CHAT EDITS



    //END CHAT EDITS

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


        public void OnGet(int collabID, string collabName)
        {
            CollabID = collabID;

            AvailableUsers = FetchAvailableUsers();
            AvailableKnowledgeItems = FetchAvailableKnowledgeItems();
            AvailablePlans = FetchAvailablePlans();
            AvailableSwots = FetchAvailableSwots();
            CollaborationUsers = FetchCollaborationUsers();
        }


        private List<SelectListItem> FetchCollaborationUsers()
        {
            CollaborationUsers = new List<SelectListItem>();
            string sqlQuery = $@"
                SELECT UserData.UserID, (UserData.FirstName + ' ' + UserData.LastName) AS FullName 
                FROM UserData 
                JOIN CollabUser ON UserData.UserID = CollabUser.UserID 
                WHERE CollabUser.CollabID = {CollabID};
            ";

            using (var reader = DBClass.GeneralReaderQuery(sqlQuery))
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

        private List<SelectListItem> FetchAvailableUsers()
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

        public IActionResult OnPostAddUser(int UserID)
        {
            string query = $"INSERT INTO CollabUser (UserID, CollabID) VALUES ({UserID}, {CollabID})";
            DBClass.InsertQuery(query);
            return RedirectToPage("./CollabDashboard", new { collabID = CollabID});
        }

        public IActionResult OnPostAddKnowledgeItem(int KnowledgeItemID)
        {
            string query = $"INSERT INTO CollabKnowledge (KnowledgeItemID, CollabID) VALUES ({KnowledgeItemID}, {CollabID})";
            DBClass.InsertQuery(query);
            return RedirectToPage("./CollabDashboard", new { collabID = CollabID});
        }

        public IActionResult OnPostAddPlan(int PlanID)
        {
            string query = $"INSERT INTO CollabPlan (PlanID, CollabID) VALUES ({PlanID}, {CollabID})";
            DBClass.InsertQuery(query);
            return RedirectToPage("./CollabDashboard", new { collabID = CollabID});
        }

        public IActionResult OnPostAddSwot(int SwotID)
        {
            string query = $"INSERT INTO CollabSwot (SwotID, CollabID) VALUES ({SwotID}, {CollabID})";
            DBClass.InsertQuery(query);
            return RedirectToPage("./CollabDashboard", new { collabID = CollabID});
        }

        public IActionResult OnPostAddChat()
        {
            string query = $"INSERT INTO Chat (Content, UserID, CollabID) VALUES ('{NewChat.Content}', {NewChat.UserID}, {CollabID})";
            DBClass.InsertQuery(query);
            return RedirectToPage("./CollabDashboard", new { collabID = CollabID});
        }
    }
}