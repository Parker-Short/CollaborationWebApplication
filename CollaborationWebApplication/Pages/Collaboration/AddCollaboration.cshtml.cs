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
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login/DBLogin");
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
                return Page();
            }


            string sqlInsertQuery = $"INSERT INTO Collaboration (CollabName) VALUES ('{NewCollab.CollabName}')";
            DBClass.InsertQuery(sqlInsertQuery);

            return RedirectToPage("Index");
        }
    }
}
