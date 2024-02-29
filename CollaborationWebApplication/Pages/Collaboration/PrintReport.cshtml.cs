using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CollaborationWebApplication.Pages.Collaboration
{
    public class PrintReportModel : PageModel
    {

        [BindProperty]
        public int CollabID { get; set; }


        public List<SelectListItem> AvailablePlans { get; set; }
        public List<SelectListItem> AvailableSwots { get; set; }

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

        public void OnGet(int collabID)
        {

            CollabID = collabID;

        }


        private List<SelectListItem> FetchAvailablePlans()
        {
            var plans = new List<SelectListItem>();
            string sqlQuery = $@"
                SELECT PlanID, PlanName
                FROM PlanData
                WHERE PlanID NOT IN (
                    SELECT PlanID
                    FROM CollabPlan
                    WHERE CollabID = {CollabID}
                );";

            using (var reader = DBClass.GeneralReaderQuery(sqlQuery))
            {
                while (reader.Read())
                {
                    plans.Add(new SelectListItem
                    {
                        Value = reader["PlanID"].ToString(),
                        Text = reader["PlanName"].ToString()
                    });
                }
            }
            return plans;
        }


        private List<SelectListItem> FetchAvailableSwots()
        {
            List<SelectListItem> swots = new List<SelectListItem>();
            string sqlQuery = "SELECT SwotID, (Strength + ', ' + Weakness + ', ' + Opportunity + ', ' + Threat) AS SwotSummary FROM Swot WHERE SwotID NOT IN (SELECT SwotID FROM CollabSwot WHERE CollabID = @CollabID);";
            using (var reader = DBClass.GeneralReaderQuery(sqlQuery.Replace("@CollabID", CollabID.ToString())))
            {
                while (reader.Read())
                {
                    swots.Add(new SelectListItem { Value = reader["SwotID"].ToString(), Text = reader["SwotSummary"].ToString() });
                }
            }
            return swots;
        }


    }
}
