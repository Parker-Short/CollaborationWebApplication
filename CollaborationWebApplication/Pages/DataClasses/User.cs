using System.ComponentModel.DataAnnotations;

namespace CollaborationWebApplication.Pages.DataClasses
{
    public class User
    {
        public int UserID { get; set; }
        [Required]
        public String? FirstName { get; set; }
        [Required]
        public String? LastName { get; set; }
        [Required]
        public String? Email { get; set; }
        [Required]
        public String? Phone { get; set; }
        [Required]
        public String? Address { get; set; }
    }
}
