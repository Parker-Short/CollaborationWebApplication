using CollaborationWebApplication.Pages.DataClasses;
using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CollaborationWebApplication.Pages.Plans
{
    public class ViewPlanModel : PageModel
    {
        // Properties for passing PlanID and Name to this page
        [BindProperty]
        public int PlanID { get; set; }

        [BindProperty]
        public string PlanName { get; set; }

        // Method to initialize the page with data for an existing plan

        //check session
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
        //check session end.

        public IActionResult OnGet(int PlanID, string PlanName)
        {
            // Perform session check at the beginning of OnGet
            var sessionCheckResult = OnGetSessionCheck();
            if (sessionCheckResult is not PageResult)
            {
                return sessionCheckResult; // Redirects if session check fails
            }
            //end session check start normal code
            this.PlanID = PlanID;
            this.PlanName = PlanName; // Assign the PlanName parameter to the Name property

            return Page();
        }

        // Property for capturing new step details from the form
        [BindProperty]
        public PlanStep NewStep { get; set; }

        // Method for handling the form submission to add a new step
        public IActionResult OnPostAddStep(int PlanID, string PlanName)
        {
            string query = $"INSERT INTO PlanStep (PlanID, StepNumber, StepName, StepDetail) VALUES ({NewStep.PlanID}, '{NewStep.StepNumber}', '{NewStep.StepName}', '{NewStep.StepDetail}')";
            try
            {
                DBClass.InsertQuery(query);
                // Redirect back to the ViewPlan page, passing along the necessary parameters
                return RedirectToPage("./ViewPlan", new { PlanID = NewStep.PlanID, PlanName = PlanName });
            }
            catch (Exception ex)
            {
                // Handle exceptions, potentially logging them or displaying an error message
                return Page(); // Stays on the current page in case of an error
            }
        }
    }
}
