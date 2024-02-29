using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Microsoft.AspNetCore.Http;
using CollaborationWebApplication.Pages.DB; // Ensure correct namespace for DB access

namespace CollaborationWebApplication.Pages.Datasets
{
    public class ViewDataModel : PageModel
    {
        public string FileName { get; private set; }
        public DataTable Data { get; private set; }

        public IActionResult OnGetSessionCheck()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login/HashedLogin");
            }
            else
            {
                return Page();
            }
        }
        public void OnGet(string fileName)
        {
            FileName = fileName;
            Data = DBClass.FetchDataForTable(fileName); // Ensure method safely queries the database
        }
    }
}
