using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic; // For List<T>
using CollaborationWebApplication.Pages.DataClasses;
using CollaborationWebApplication.Pages.DB;
using System;

namespace CollaborationWebApplication.Pages.KnowledgeItems
{
    public class AddKnowledgeItemsModel : PageModel
    {
        [BindProperty]
        public KnowledgeItem NewKnowledgeItem { get; set; }

        public List<SelectListItem> KnowledgeItemCategory { get; set; }

        //check session
        public IActionResult OnGetSessionCheck()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must log in to access that page!");
                return RedirectToPage("/Login/HashedLogin");
            }
            else
            {
                return Page();
            }
        }

        public IActionResult OnGet()
        {
            var sessionCheckResult = OnGetSessionCheck();
            if (sessionCheckResult is not PageResult)
            {
                return sessionCheckResult; // Redirects if session check fails
            }

            KnowledgeItemCategory = new List<SelectListItem>();

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

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ErrorMessage"] = "Please fill out all required fields";
                return Page();
            }

            string username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/Login/HashedLogin");
            }

            int userID = DBClass.FetchUserIDForUsername(username);
            if (userID == -1)
            {
                return RedirectToPage("/Login/HashedLogin");
            }

            string sqlInsertQuery = @"
            INSERT INTO KnowledgeItem 
            (KnowledgeTitle, KnowledgeSubject, KnowledgeInformation, KMDate, KnowledgeCategoryID, UserID) 
            VALUES 
            (@KnowledgeTitle, @KnowledgeSubject, @KnowledgeInformation, @KMDate, @KnowledgeCategoryID, @UserID)";

            var parameters = new Dictionary<string, object>
            {
                { "@KnowledgeTitle", NewKnowledgeItem.KnowledgeTitle },
                { "@KnowledgeSubject", NewKnowledgeItem.KnowledgeSubject },
                { "@KnowledgeInformation", NewKnowledgeItem.KnowledgeInformation },
                { "@KMDate", DBNull.Value }, // Hardcoding KMDate as NULL
                { "@KnowledgeCategoryID", NewKnowledgeItem.KnowledgeCategoryID },
                { "@UserID", userID }
            };

            DBClass.ExecuteSqlCommand(sqlInsertQuery, parameters);
            return RedirectToPage("Index");
        }
    }
}
