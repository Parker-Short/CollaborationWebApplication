using CollaborationWebApplication.Pages.DataClasses;
using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CollaborationWebApplication.Pages.Users
{
    public class IndexModel : PageModel
    {
        public List<User> Users { get; set; }

        public IndexModel()
        {
            Users = new List<User>();
        }

        public void OnGet()
        {
            Users = new List<User>();

            // using line so we can ensure its closed right
            using (SqlDataReader userReader = DBClass.UserReader())
            {
                while (userReader.Read())
                {
                    Users.Add(new User
                    {
                        UserID = int.Parse(userReader["UserID"].ToString()),
                        FirstName = userReader["FirstName"].ToString(),
                        LastName = userReader["LastName"].ToString(),
                        Email = userReader["Email"].ToString(),
                        Phone = userReader["Phone"].ToString(),
                        Address = userReader["Address"].ToString()
                    });
                }
            } // The connection is closed auto.
        }
    }
}
