using CollaborationWebApplication.Pages.DataClasses;
using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic; // For List<T>
using System.Data.SqlClient;

namespace CollaborationWebApplication.Pages.KnowledgeItems
{
    public class AddKnowledgeItemsModel : PageModel
    {
        [BindProperty]
        public KnowledgeItem NewKnowledgeItem { get; set; }

        public List<SelectListItem> KnowledgeItemCategory { get; set; }
        public List<SelectListItem> UserList { get; set; }

        //check session
        public IActionResult OnGetSessionCheck()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login/DBLogin");
            }
            else
            {
                return Page();
            }
        }
        //check session end.

        public IActionResult OnGet()
        {
            // Perform session check at the beginning of OnGet
            var sessionCheckResult = OnGetSessionCheck();
            if (sessionCheckResult is not PageResult)
            {
                return sessionCheckResult; // Redirects if session check fails
            }
            //end session check start normal code

            KnowledgeItemCategory = new List<SelectListItem>();
            UserList = new List<SelectListItem>();

            using (var CategoryReader = DBClass.GeneralReaderQuery("SELECT * FROM KnowledgeItemCategory"))
            {
                while (CategoryReader.Read())
                {
                    KnowledgeItemCategory.Add(new SelectListItem
                    {
                        Text = CategoryReader["CategoryName"].ToString(),
                        Value = CategoryReader["KnowledgeCategoryID"].ToString()
                    });
                }
            }

            using var UserReader = DBClass.GeneralReaderQuery("SELECT * FROM UserData");
            {
                while (UserReader.Read())
                {
                    UserList.Add(new SelectListItem
                    {
                        Text = UserReader["FirstName"].ToString() + " " + UserReader["LastName"].ToString(),
                        Value = UserReader["UserID"].ToString()
                    });
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ErrorMessage"] = "Please fill out all required fields";
                return Page();
            }

            string sqlInsertQuery = $"INSERT INTO KnowledgeItem (KnowledgeTitle, KnowledgeSubject, KnowledgeInformation, KnowledgeCategoryID, UserID) VALUES ('{NewKnowledgeItem.KnowledgeTitle}', '{NewKnowledgeItem.KnowledgeSubject}', '{NewKnowledgeItem.KnowledgeInformation}', {NewKnowledgeItem.KnowledgeCategoryID}, {NewKnowledgeItem.UserID})";

            DBClass.InsertQuery(sqlInsertQuery);
            return RedirectToPage("Index");
        }
    }
}
