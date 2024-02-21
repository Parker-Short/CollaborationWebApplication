using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CollaborationWebApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace CollaborationWebApplication.Pages.Datasets
{
    public class DatasetFormModel : PageModel
    {
        [BindProperty]
        public int SelectedUserId { get; set; }

        [BindProperty]
        public int SelectedCollabId { get; set; }

        [BindProperty]
        public string CsvData { get; set; }

        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Collaborations { get; set; } = new List<SelectListItem>();

        public IActionResult OnGet()
        {
            // Check session
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login/DBLogin");
            }

            // Populate Users from UserData
            using (var userReader = DBClass.GeneralReaderQuery("SELECT UserID, FirstName, LastName FROM UserData"))
            {
                while (userReader.Read())
                {
                    Users.Add(new SelectListItem
                    {
                        Value = userReader["UserID"].ToString(),
                        Text = $"{userReader["FirstName"]} {userReader["LastName"]}"
                    });
                }
            }

            // Populate Collaborations from Collaboration table
            using (var collabReader = DBClass.GeneralReaderQuery("SELECT CollabID, CollabName FROM Collaboration"))
            {
                while (collabReader.Read())
                {
                    Collaborations.Add(new SelectListItem
                    {
                        Value = collabReader["CollabID"].ToString(),
                        Text = collabReader["CollabName"].ToString()
                    });
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Insert the CSV data into the Dataset table and capture the DatasetID
            string insertDatasetSql = $"INSERT INTO Dataset (DataValues, UserID) OUTPUT INSERTED.DatasetID VALUES ('{CsvData}', {SelectedUserId})";

            int datasetId;
            using (var reader = DBClass.GeneralReaderQuery(insertDatasetSql))
            {
                if (reader.Read())
                {
                    datasetId = Convert.ToInt32(reader["DatasetID"]);
                }
                else
                {
                    // Handle error: DatasetID not retrieved
                    return RedirectToPage("./DatasetForm", new { error = "Failed to insert dataset" });
                }
            }

            // Insert into CollabData table if we have a valid DatasetID
            if (datasetId > 0)
            {
                string insertCollabDataSql = $"INSERT INTO CollabData (CollabID, DatasetID) VALUES ({SelectedCollabId}, {datasetId})";
                DBClass.InsertQuery(insertCollabDataSql);
            }

            return RedirectToPage("./DatasetForm"); // Redirect to your intended success page
        }
    }
}
