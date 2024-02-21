using System.ComponentModel.DataAnnotations;

namespace CollaborationWebApplication.Pages.DataClasses
{
    public class Swot
    {
        public int SwotID { get; set; }
        [Required]
        public String? SwotName { get; set; }
        [Required]
        public String? Strength { get; set; }
        [Required]
        public String? Weakness { get; set; }
        [Required]
        public String? Opportunity { get; set; }
        [Required]
        public String? Threat { get; set; }
        [Required]
        public int UserID { get; set; }

    }
}
