using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace CollaborationWebApplication.Pages.FileUploads
{
    public class FileUploadModel : PageModel
    {
        [BindProperty]
        public List<IFormFile> FileList { get; set; }

        public void OnGet()
        {
        // Example adapted from:
        // https://code-maze.com/file-upload-aspnetcore-mvc/
        }

        public IActionResult OnPost()
        {

            var filePaths = new List<string>();
            foreach (var formFile in FileList)
            {
                if (formFile.Length > 0)
                {
                    // full path to file in temp location
                    var filePath = Directory.GetCurrentDirectory() + @"\wwwroot\fileupload\" + formFile.FileName;
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                }
            }


            return RedirectToPage("FileHandling");
        }
    }
}
