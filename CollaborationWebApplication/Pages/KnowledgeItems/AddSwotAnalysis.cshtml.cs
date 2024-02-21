using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using CollaborationWebApplication.Pages.DB;
using CollaborationWebApplication.Pages.DataClasses;

namespace CollaborationWebApplication.Pages.KnowledgeItems
{
    public class AddSwotAnalysisModel : PageModel
    {
        [BindProperty]
        public Swot NewSwot { get; set; }

        [BindProperty]
        public int UserID { get; set; }

        public List<SelectListItem> UserList { get; set; }

        //check session
        public IActionResult OnGetSessionCheck()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login/DBLogin");
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
            UserList = new List<SelectListItem>();
            using (var UserReader = DBClass.GeneralReaderQuery("SELECT * FROM UserData"))
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

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ErrorMessage"] = "Please fill out all required fields";
                return Page();
            }

            // Parameterized SQL Insert Query
            string sqlInsertSwot = @"
            INSERT INTO Swot 
            (SwotName, Strength, Weakness, Opportunity, Threat, UserID) 
            VALUES 
            (@SwotName, @Strength, @Weakness, @Opportunity, @Threat, @UserID)";

            // Create a dictionary for the parameters
            var parameters = new Dictionary<string, object>
            {
                { "@SwotName", NewSwot.SwotName },
                { "@Strength", NewSwot.Strength },
                { "@Weakness", NewSwot.Weakness },
                { "@Opportunity", NewSwot.Opportunity },
                { "@Threat", NewSwot.Threat },
                { "@UserID", UserID }
            };

            // Execute the SQL command with parameters
            DBClass.ExecuteSqlCommand(sqlInsertSwot, parameters);

            return RedirectToPage("Index");
        }

    }
}
