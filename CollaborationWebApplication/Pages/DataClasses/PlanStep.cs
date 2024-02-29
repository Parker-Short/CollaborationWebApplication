using System.ComponentModel.DataAnnotations;

namespace CollaborationWebApplication.Pages.DataClasses
{
    public class PlanStep // Declaring variables
    {
        public int PlanID { get; set; }
        [Required]
        public int StepNumber { get; set; }
        [Required]
        public string StepName { get; set; }
        [Required]
        public string StepDetail { get; set; }
    }
}
