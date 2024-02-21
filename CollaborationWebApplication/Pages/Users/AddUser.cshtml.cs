using CollaborationWebApplication.Pages.DataClasses;
using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CollaborationWebApplication.Pages.Users
{
    public class AddUserModel : PageModel
    {
        [BindProperty]
        public User NewUser { get; set; }

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
        //Inserts the data to the DB by calling DBclass Methods
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ErrorMessage"] = "Please fill out all required fields";
                return Page();
            }
            else
            {
                DBClass.InsertUserData(NewUser);
                return RedirectToPage("Index");
            }


        }

        public IActionResult OnPostPopulateHandler()
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear(); //Causes Model Validation to be skipped & reset for the next entry
                //Code to populate form with:
                NewUser.FirstName = "John";
                NewUser.LastName = "Apple";
                NewUser.Email = "JohnApple@gmail.com";
                NewUser.Phone = "1234567890";
                NewUser.Address = "123 Carrier Drive";
                // END code population stuff
            }
            return Page();
        }
    }
}
