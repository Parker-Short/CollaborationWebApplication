using CollaborationWebApplication.Pages.DataClasses;
using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic; // For List<T>
using System.Data.SqlClient;

namespace CollaborationWebApplication.Pages.KnowledgeItems
{
    public class AddKnowledgeItemsModel : PageModel
    {
        [BindProperty]
        public KnowledgeItem NewKnowledgeItem { get; set; }

        public List<SelectListItem> KnowledgeItemCategory { get; set; }
        public List<SelectListItem> UserList { get; set; }

        //check session
        public IActionResult OnGetSessionCheck()
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
        //check session end.

        public IActionResult OnGet()
        {
            // Perform session check at the beginning of OnGet
            var sessionCheckResult = OnGetSessionCheck();
            if (sessionCheckResult is not PageResult)
            {
                return sessionCheckResult; // Redirects if session check fails
            }
            //end session check start normal code

            KnowledgeItemCategory = new List<SelectListItem>();
            UserList = new List<SelectListItem>();

            using (var CategoryReader = DBClass.GeneralReaderQuery("SELECT * FROM KnowledgeItemCategory"))
            {
                while (CategoryReader.Read())
                {
                    KnowledgeItemCategory.Add(new SelectListItem
                    {
                        Text = CategoryReader["CategoryName"].ToString(),
                        Value = CategoryReader["KnowledgeCategoryID"].ToString()
                    });
                }
            }

            using var UserReader = DBClass.GeneralReaderQuery("SELECT * FROM UserData");
            {
                while (UserReader.Read())
                {
                    UserList.Add(new SelectListItem
                    {
                        Text = UserReader["FirstName"].ToString() + " " + UserReader["LastName"].ToString(),
                        Value = UserReader["UserID"].ToString()
                    });
                }
            }

            return Page();
        }

        public IActionResult OnPostAddKnowledgeItemHandler()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ErrorMessage"] = "Please fill out all required fields";
                return Page();
            }

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

            // Parameterized SQL Insert Query
            string sqlInsertQuery = @"
            INSERT INTO KnowledgeItem 
            (KnowledgeTitle, KnowledgeSubject, KnowledgeInformation, KnowledgeCategoryID, UserID) 
            VALUES 
            (@KnowledgeTitle, @KnowledgeSubject, @KnowledgeInformation, @KnowledgeCategoryID, @UserID)";

            // Create a dictionary for the parameters
            var parameters = new Dictionary<string, object>
            {
                { "@KnowledgeTitle", NewKnowledgeItem.KnowledgeTitle },
                { "@KnowledgeSubject", NewKnowledgeItem.KnowledgeSubject },
                { "@KnowledgeInformation", NewKnowledgeItem.KnowledgeInformation },
                { "@KnowledgeCategoryID", NewKnowledgeItem.KnowledgeCategoryID },
                { "@UserID", userID }
            };

            // Execute the SQL command with parameters
            DBClass.ExecuteSqlCommand(sqlInsertQuery, parameters);
            return RedirectToPage("Index");
        }


    }
}
