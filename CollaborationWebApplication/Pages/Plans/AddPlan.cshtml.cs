using CollaborationWebApplication.Pages.DataClasses;
using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;
using System.Collections.Generic;


namespace CollaborationWebApplication.Pages.Plans
{
    public class AddPlanModel : PageModel
    {
        [BindProperty]
        public Plan NewPlan { get; set; }

        // This will hold the users for the dropdown
        public List<SelectListItem> Users { get; set; }

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
            Users = new List<SelectListItem>();

            // Query to get all users from the DB
            var sqlQuery = "SELECT UserID, FirstName + ' ' + LastName AS UserName FROM UserData";
            using (var reader = DBClass.GeneralReaderQuery(sqlQuery))
            {
                while (reader.Read())
                {
                    // Adds users to dropdown list
                    Users.Add(new SelectListItem
                    {
                        Value = reader["UserID"].ToString(),
                        Text = reader["UserName"].ToString()
                    });
                }
            }

            return Page();
        }



        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
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

            string sqlInsertQuery = $"INSERT INTO PlanData (PlanName, Content, UserID) VALUES ( '{NewPlan.PlanName}', '{NewPlan.Content}', {userID})";

            DBClass.InsertQuery(sqlInsertQuery);

            return RedirectToPage("Index");
        }  
    }
}
