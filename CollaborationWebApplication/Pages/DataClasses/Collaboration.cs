using System.ComponentModel.DataAnnotations;

namespace CollaborationWebApplication.Pages.DataClasses
{
    public class Collaboration // Declaring variables
    {
        public int CollaborationID { get; set; }
        [Required]
        public string CollabName { get; set; }
        
        public int UserID { get; set; }
        
        public int KnowledgeItemID { get; set; }
        
        public int DatasetID { get; set; }

        public int AnalysisID { get; set; }

        public int PlanID { get; set; }
    }
}
