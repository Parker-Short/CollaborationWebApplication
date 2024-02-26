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
                return RedirectToPage("/Login/HashedLogin");
            }
            else
            {
                return null;
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ErrorMessage"] = "Please fill out all required fields";
                return Page();
            }
            else
            {



                // Construct the parameterized SQL query
                string sqlQuery = @"
            INSERT INTO UserData (FirstName, LastName, Email, Phone, Address) 
            VALUES (@FirstName, @LastName, @Email, @Phone, @Address)";

                // Create a dictionary for the parameters
                var parameters = new Dictionary<string, object>
        {
            { "@FirstName", NewUser.FirstName },
            { "@LastName", NewUser.LastName },
            { "@Email", NewUser.Email },
            { "@Phone", NewUser.Phone },
            { "@Address", NewUser.Address }
        };

                // Execute the SQL command with parameters
                DBClass.ExecuteSqlCommand(sqlQuery, parameters);

                return RedirectToPage("Index");
            }
        }

    }
}
