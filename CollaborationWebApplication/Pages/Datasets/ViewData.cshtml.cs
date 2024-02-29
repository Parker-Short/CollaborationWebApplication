using System.Data;
using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CollaborationWebApplication.Pages.DB;

// Ensure this using directive matches your actual namespace
namespace CollaborationWebApplication.Pages.Datasets
{
    public class ViewDataModel : PageModel
    {
        public DataTable DatasetData { get; set; }
        public string TableName { get; set; }

        public void OnGet(string tableName)
        {
            TableName = tableName;
            // Assuming DBClass.FetchDataForTable safely fetches data for the given table name
            DatasetData = DBClass.FetchDataForTable(tableName);
        }
    }

}
