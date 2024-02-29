using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Data.SqlClient;
using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Http;

namespace CollaborationWebApplication.Pages.FileUploads
{
    public class FileUploadModel : PageModel
    {
        [BindProperty]
        public List<IFormFile> FileList { get; set; }

        public IActionResult OnGetSessionCheck()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login/HashedLogin");
            }
            else
            {
                return Page();
            }
        }

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
