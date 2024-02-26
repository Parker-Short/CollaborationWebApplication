using CollaborationWebApplication.Pages.DataClasses;
using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CollaborationWebApplication.Pages.Login
{
    public class CreateHashedLoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [Required]
        [BindProperty]
        public string Password { get; set; }
        [Required]

        [BindProperty]
        public User NewUser { get; set; }

        public void OnGet()
        {
        }


        public IActionResult OnPost()
        {
            // Perform Validation First on Form
            if (!ModelState.IsValid)
            {
                return Page(); // Return with validation errors
            }

            // Check if the username already exists
            string checkUsernameQuery = $"SELECT COUNT(*) FROM HashedCredentials WHERE Username = '{Username}'"; 
            using (var reader = DBClass.GeneralReaderQueryAUTH(checkUsernameQuery))
            {
                if (reader.Read() && (int)reader[0] > 0)
                {
                    ViewData["UsernameError"]= "Username already exists. Please choose a different one.";
                    return Page(); // Username exists, return with error
                }
            }

            try
            {
                DBClass.CreateHashedUser(Username, Password, NewUser.FirstName, NewUser.LastName, NewUser.Email, NewUser.Phone, NewUser.Address);
                return RedirectToPage("HashedLogin");
            }
            catch (Exception ex)
            {
            
                return Page(); // Return to the page with a generic error message
            }
        }

    }
}
