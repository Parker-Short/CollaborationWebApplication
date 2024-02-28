using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace CollaborationWebApplication.Pages.FileUploads
{
    public class FileUploadModel : PageModel
    {
        [BindProperty]
        public List<IFormFile> FileList { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (var formFile in FileList)
            {
                if (formFile.Length > 0)
                {
                    if (!formFile.FileName.EndsWith(".csv"))
                    {
                        ModelState.AddModelError("FileList", "Only CSV files are allowed.");
                        return Page();
                    }

                    // full path to file in temp location
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fileupload", formFile.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    // Redirect to the FileHandling page along with the path of the uploaded file
                    return RedirectToPage("FileHandling", new { filePath = filePath });
                }
            }

            // If we get here, it means no files were processed
            return Page();
        }
    }
}
