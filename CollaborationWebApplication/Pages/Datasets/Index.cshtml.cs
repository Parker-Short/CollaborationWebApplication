using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using CollaborationWebApplication.Pages.FileUploads; // Adjust the namespace as necessary

namespace CollaborationWebApplication.Pages.Datasets
{
    public class IndexModel : PageModel
    {
        public List<string> Datasets { get; set; }

        public IActionResult OnGet()
        {
            // Check if the user is logged in by looking for "username" in the session
            if (HttpContext.Session.GetString("username") == null)
            {
                // If "username" is not found, set an error message and redirect to the login page
                HttpContext.Session.SetString("LoginError", "You must log in to access that page!");
                return RedirectToPage("/Login/HashedLogin");
            }
            else
            {
                // User is logged in, proceed with loading datasets
                Datasets = FileHandlingModel.FetchAllDatasets(); // Adapt method to fetch dataset names
                return Page(); // Continue to render the current page
            }
        }
    }
}
