using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using CollaborationWebApplication.Pages.FileUploads; // Adjust namespace as necessary

namespace CollaborationWebApplication.Pages.Datasets
{
    public class IndexModel : PageModel
    {
        public List<string> Datasets { get; set; }


        public void OnGet()
        {
            Datasets = FileHandlingModel.FetchAllDatasets(); // Adapt method to fetch dataset names
        }
    }
}
