using System.ComponentModel.DataAnnotations;

namespace CollaborationWebApplication.Pages.DataClasses
{
    public class Chat // Decalring varibales 
    {

        public int? ChatId { get; set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public int? UserID { get; set; }
        [Required]
        public int? CollabID { get; set; }
    }
}
