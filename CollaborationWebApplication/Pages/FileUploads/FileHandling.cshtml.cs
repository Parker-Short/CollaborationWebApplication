using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CollaborationWebApplication.Pages.FileUploads
{
    public class FileHandlingModel : PageModel
    {
        [BindProperty]
        public List<IFormFile> FileList { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var csvDataList = new List<string>();

            foreach (var formFile in FileList)
            {
                if (formFile.Length > 0)
                {
                    using (var reader = new StreamReader(formFile.OpenReadStream()))
                    {
                        string content = await reader.ReadToEndAsync();
                        csvDataList.Add(content);
                    }
                }
            }

            // Store the CSV data in TempData. If multiple files are uploaded, you might need to handle them separately.
            //TempData["CsvData"] = JsonConvert.SerializeObject(csvDataList);

            return RedirectToPage("/Datasets/DatasetForm");
        }

    }
    
}
