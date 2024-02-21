using System.ComponentModel.DataAnnotations;

namespace CollaborationWebApplication.Pages.DataClasses
{
    public class Plan
    {
        [Required]
        public String? PlanName { get; set; }
        [Required]
        public String? Content { get; set; }
        [Required]
        public int UserID { get; set; }
    }
}
