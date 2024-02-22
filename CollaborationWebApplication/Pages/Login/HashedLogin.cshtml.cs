using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CollaborationWebApplication.Pages.Login
{
    public class HashedLoginModel : PageModel
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
            if (DBClass.HashedParameterLogin(Username, Password))
            {
                HttpContext.Session.SetString("username", Username);
                ViewData["LoginMessage"] = "Login Successful!";
                DBClass.CollabAppConnection.Close();
                return Page();
            }
            else
            {
                ViewData["LoginMessage"] = "Username and/or Password Incorrect";
                DBClass.CollabAppConnection.Close();
                return Page();
            }

        }
    }
}
