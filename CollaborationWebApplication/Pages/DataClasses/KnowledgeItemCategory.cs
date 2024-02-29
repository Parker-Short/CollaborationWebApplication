using System.ComponentModel.DataAnnotations;

namespace CollaborationWebApplication.Pages.DataClasses
{
    public class KnowledgeItemCategory // declaring variables
    {
        public int KnowledgeCategoryID { get; set; }
        [Required]
        public String? CategoryName { get; set; }
    }
}
