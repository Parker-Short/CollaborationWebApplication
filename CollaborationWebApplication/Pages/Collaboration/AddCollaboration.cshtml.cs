using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CollaborationWebApplication.Pages.DB;

namespace CollaborationWebApplication.Pages.Collaboration
{
    public class AddCollaborationModel : PageModel
    {
        [BindProperty]
        public DataClasses.Collaboration NewCollab { get; set; }

        //check session
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") == null) // Check if username is in session
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login/HashedLogin");


            }
            else
            {
                return null;
            }
        }
        //check session end.

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // return page with validation errors
            }
            else
            {
                string sqlQuery = @"
                INSERT INTO Collaboration (CollabName) VALUES (@CollabName)";

                var parameters = new Dictionary<string, object> // parameters for the sql query 
                {
                    {"@CollabName", NewCollab.CollabName } // set the value of collab name
                };

                DBClass.ExecuteSqlCommand(sqlQuery, parameters);

                return RedirectToPage("Index"); // return back to the index page


            }
        }
    }
}
