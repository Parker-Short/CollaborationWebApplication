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
                return RedirectToPage("/Login/DBLogin");
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
            }

            DBClass.InsertKnowledgeCategory(NewKnowledgeItemCategory);
            return RedirectToPage("Index"); //Returns user to the main page for Knowledge Items
        }
    }
}
