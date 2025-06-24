using IronXL;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace IronSofts.Controllers
{
    public class ExcelController : Controller
    {


        [HttpGet("/excel/create")]
        public IActionResult CreateExcel()
        {
            var workbook = WorkBook.Create(ExcelFileFormat.XLSX);
            var sheet = workbook.CreateWorkSheet("Report");
           


            // Header row
            sheet["A1"].Value = "ID";
            sheet["B1"].Value = "Name";
            sheet["C1"].Value = "Email";

            // Data rows
            sheet["A2"].Value = 1;
            sheet["B2"].Value = "Aryan";
            sheet["C2"].Value = "aryan@example.com";

            sheet["A3"].Value = 2;
            sheet["B3"].Value = "Neha";
            sheet["C3"].Value = "neha@example.com";

            //sheet["D1"].Value = "Total";
            //sheet["D2"].Formula = "=SUM(A2:A3)";


            // Save and return as download
            string path = "wwwroot/files/created.xlsx";
            workbook.SaveAs(path);

           


            return File(System.IO.File.ReadAllBytes(path), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "created.xlsx");
        }

        [HttpGet("/excel/read")]
        public IActionResult ReadExcel()
        {
            var workbook = WorkBook.Load("wwwroot/files/created.xlsx");
            var sheet = workbook.DefaultWorkSheet;

            string result = "";
            foreach (var row in sheet.Rows)
            {
                result += string.Join(" | ", row.Select(c => c.Value?.ToString())) + "\n";
            }

            return Content(result);
        }

        [HttpGet("/excel/pdf")]
        public IActionResult ConvertExcelToPdf()
        {
            var workbook = WorkBook.Load("wwwroot/files/created.xlsx");

            string html = workbook.ExportToHtmlString();

            var renderer = new HtmlToPdf();
            var pdf = renderer.RenderHtmlAsPdf(html);

            string pdfPath = "wwwroot/files/exported.pdf";
            pdf.SaveAs(pdfPath);

            return File(System.IO.File.ReadAllBytes(pdfPath), "application/pdf", "exported.pdf");
        }



    }
}
