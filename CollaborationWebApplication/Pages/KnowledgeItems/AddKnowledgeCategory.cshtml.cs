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

        //check session
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

        public IActionResult OnPost()

        {
            if (!ModelState.IsValid)
            {
                return Page();
                ViewData["ErrorMessage"] = "Please enter a category";
            }
            else
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
