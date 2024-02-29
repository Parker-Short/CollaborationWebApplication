using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CollaborationWebApplication.Pages.Datasets
{
    public class Index1Model : PageModel
    {

        public List<string> FileNames { get; set; }

        public void OnGet()
        {
            using (var FileReader = DBClass.GeneralReaderQuery("SELECT * FROM Dataset"))
            {
                while (FileReader.Read())
                {
                    FileNames.Add(FileReader["FileName"].ToString());
                }
            }
        }
    }
}
