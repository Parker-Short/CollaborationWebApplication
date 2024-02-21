using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CollaborationWebApplication.Pages.Login
{
    public class DBLoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnGet(String logout)
        {
            if (logout == "true")
            {
                HttpContext.Session.Clear();
                ViewData["LoginMessage"] = "Successfully Logged Out!";
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            string loginQuery = "SELECT COUNT(*) FROM Credentials where Username = '";
            loginQuery += Username + "' and Password='" + Password + "'";

            if (DBClass.LoginQuery(loginQuery) > 0)
            {
                HttpContext.Session.SetString("username", Username);
                DBClass.CollabAppConnection.Close();

                return RedirectToPage("../Index");

            }
            else
            {
                ViewData["LoginMessage"] = "Username and/or Password Incorrect";
                DBClass.CollabAppConnection.Close();
                return Page();

            }


        }

        public IActionResult OnPostLogoutHandler()
        {
            HttpContext.Session.Clear();
            return Page();
        }
    }
}
