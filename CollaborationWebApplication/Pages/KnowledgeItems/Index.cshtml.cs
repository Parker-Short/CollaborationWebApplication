using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CollaborationWebApplication.Pages.KnowledgeItems
{
    public class IndexModel : PageModel
    {
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
    }
}
