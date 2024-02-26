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

  
        public IActionResult OnGet(String logout)
        {
            if (logout == "true")
            {
                HttpContext.Session.Clear();

            }

            return Page();
        }


        public IActionResult OnPostLoginHandler()
        {
            if (DBClass.HashedParameterLogin(Username, Password))
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

        //public IActionResult OnPostLoginHandler()
        //{
        //    if (DBClass.HashedParameterLogin(Username, Password))
        //    {
        //        // After successful login, fetch the UserID for the Username
        //        var userID = DBClass.FetchUserIDForUsername(Username); // Implement this method

        //        // Store UserID and Username in the session
        //        HttpContext.Session.SetInt32("UserID", userID);
        //        HttpContext.Session.SetString("Username", Username);

        //        return RedirectToPage("../Index");
        //    }
        //    else
        //    {
        //        ViewData["LoginMessage"] = "Username and/or Password Incorrect";
        //        return Page();
        //    }
        //}


        public IActionResult OnPostLogoutHandler()
        {
            HttpContext.Session.Clear();
            return Page();
        }

    }
}
