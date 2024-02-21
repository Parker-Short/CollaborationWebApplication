using System.ComponentModel.DataAnnotations;

namespace CollaborationWebApplication.Pages.DataClasses
{
    public class Dataset
    {
           
        public int DatasetID { get; set; }
        [Required]
        public String? DataValues { get; set; }
    }
}
