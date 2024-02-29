using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborationWebApplication.Pages.FileUploads
{
    public class FileUploadModel : PageModel
    {
        [BindProperty]
        public List<IFormFile> FileList { get; set; }

        public IActionResult OnGet()
        {
            // Session check for accessing the page
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login/HashedLogin");
            }

            return Page(); // Proceed if the user is logged in
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Session check for form submission
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login/HashedLogin");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (var formFile in FileList)
            {
                if (formFile.Length > 0)
                {
                    // Ensure only CSV files are uploaded
                    if (!formFile.FileName.EndsWith(".csv"))
                    {
                        ModelState.AddModelError("FileList", "Only CSV files are allowed.");
                        return Page();
                    }

                    // Full path to file in temp location
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
