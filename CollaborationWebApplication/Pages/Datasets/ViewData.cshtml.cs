using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Microsoft.AspNetCore.Http;
using CollaborationWebApplication.Pages.DB; // Adjust if necessary for correct DB access

namespace CollaborationWebApplication.Pages.Datasets
{
    public class ViewDataModel : PageModel
    {
        public string FileName { get; private set; }
        public DataTable Data { get; private set; }

        public IActionResult OnGet(string fileName)
        {
            // Check if the user is logged in by looking for "username" in the session
            if (HttpContext.Session.GetString("username") == null)
            {
                // If "username" is not found, set an error message and redirect to the login page
                HttpContext.Session.SetString("LoginError", "You must log in to access that page!");
                return RedirectToPage("/Login/HashedLogin");
            }

            // User is logged in, proceed with fetching data
            FileName = fileName;
            Data = DBClass.FetchDataForTable(fileName); // Make sure this method safely queries the database

            return Page(); // Continue to render the current page
        }
    }
}
