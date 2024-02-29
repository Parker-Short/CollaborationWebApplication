using System.ComponentModel.DataAnnotations;

namespace CollaborationWebApplication.Pages.DataClasses
{
    public class Dataset //Declaring variables
    {
           
        public int DatasetID { get; set; }
        [Required]
        public String? DataValues { get; set; }
    }
}
