using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CollaborationWebApplication.Pages.Login
{
    public class SecureLoginModel : PageModel
    {

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {

            if (DBClass.SecureLogin(Username, Password) > 0)
            {
                HttpContext.Session.SetString("username", Username);
                ViewData["LoginMessage"] = "Login successful";
            }
            else
            {
                ViewData["LoginMessage"] = "Wot happened";
            }

            DBClass.CollabAppConnection.Close();

            return RedirectToPage("../Index");
        }
    }
}
