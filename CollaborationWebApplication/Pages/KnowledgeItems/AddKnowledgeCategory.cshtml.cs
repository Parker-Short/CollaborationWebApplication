using CollaborationWebApplication.Pages.DataClasses;
using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CollaborationWebApplication.Pages.KnowledgeItems
{
    public class AddKnowledgeCategoryModel : PageModel
    {
        [BindProperty]
        public KnowledgeItemCategory NewKnowledgeItemCategory { get; set; }

        // Check if the user session is active
        public IActionResult OnGet()
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

        // Handle POST request when adding a new knowledge category
        public IActionResult OnPost()

        {
            if (!ModelState.IsValid) // Category must be entered first
            {
                return Page();
                ViewData["ErrorMessage"] = "Please enter a category";
            }
            else // If the model state is valid, insert the new knowledge category into the database
            {
                string sqlQuery = @"
                INSERT INTO KnowledgeItemCategory (CategoryName) VALUES (@CategoryName)";

                var parameters = new Dictionary<string, object>
                {
                    {"@CategoryName", NewKnowledgeItemCategory.CategoryName }
                };

                DBClass.ExecuteSqlCommand(sqlQuery, parameters);

                return RedirectToPage("Index");
            }

        
        }
    }
}
