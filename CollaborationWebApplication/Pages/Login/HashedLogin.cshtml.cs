using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CollaborationWebApplication.Pages.Login
{
    public class HashedLoginModel : PageModel
    {
        [Required]
        [BindProperty]
        public string Username { get; set; }
        [Required]
        [BindProperty]
        public string Password { get; set; }
        

        // Handles GET requests, particularly for logging out
        public IActionResult OnGet(String logout)
        {
            if (logout == "true")
            { // If logout query parameter is true, clear session
                HttpContext.Session.Clear();

            }

            return Page();
        }

        // Handles POST requests for login
        public IActionResult OnPostLoginHandler()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (DBClass.HashedParameterLogin(Username, Password)) // Validate username and password against hashed credentials in the database
            {
                HttpContext.Session.SetString("username", Username);
                ViewData["LoginMessage"] = "Login Successful!";
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

        // Handles POST requests for logout
        public IActionResult OnPostLogoutHandler()
        {
            HttpContext.Session.Clear();
            return Page();
        }

    }
}
