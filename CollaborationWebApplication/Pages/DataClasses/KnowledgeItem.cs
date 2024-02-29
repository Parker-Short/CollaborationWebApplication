using System.ComponentModel.DataAnnotations;
namespace CollaborationWebApplication.Pages.DataClasses
{
    public class KnowledgeItem // Declaring variables
    {

        public int KnowledgeItemID { get; set; }
        [Required]
        public String? KnowledgeTitle { get; set; }
        [Required]
        public String? KnowledgeSubject { get; set; }
        [Required]
        public String? KnowledgeInformation { get; set; }
        [Required]
        public int KnowledgeCategoryID { get; set; }
        [Required]
        public int UserID { get; set; }

    }
}
