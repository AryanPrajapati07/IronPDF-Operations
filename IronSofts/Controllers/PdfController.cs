using IronPdf.Editing;
using IronPdf.Security;
using IronSofts.Services;
using Microsoft.AspNetCore.Mvc;


namespace IronSofts.Controllers
{
    public class PdfController : Controller
    {
        private readonly IViewRenderService _viewRenderService;

        public PdfController(IViewRenderService viewRenderService)
        {
            _viewRenderService = viewRenderService;
        }

        [HttpGet("/pdf/operations")]
        public IActionResult Operations()
        {
            return View();
        }

        //pdf generate
        [HttpGet("/pdf/generate")]
        public async Task<IActionResult> Generate()
        {
            var html = await _viewRenderService.RenderToStringAsync("Pdf/Template", new { Name = "Aryan", Date = DateTime.Now });
            var renderer = new HtmlToPdf();
            var pdf = renderer.RenderHtmlAsPdf(html);
            return File(pdf.BinaryData, "application/pdf", "Report.pdf");
        }

        // pdf merge
        [HttpGet("/pdf/merge")]
        public IActionResult MergePdfs()
        {
            var pdf1 = PdfDocument.FromFile("wwwroot/pdfs/file1.pdf");
            var pdf2 = PdfDocument.FromFile("wwwroot/pdfs/file2.pdf");
            var merged = PdfDocument.Merge(pdf1, pdf2);
            return File(merged.BinaryData, "application/pdf", "Merged.pdf");
        }

        // split pdf pages
        [HttpGet("/pdf/split")]
        public IActionResult SplitPdf()
        {
            var pdf = PdfDocument.FromFile("wwwroot/pdfs/multipage.pdf");

            // Fix: Replace the non-existent SplitToIndividualPages method with a valid approach
            var splitPages = new List<PdfDocument>();
            for (int i = 0; i < pdf.PageCount; i++)
            {
                var singlePagePdf = pdf.CopyPage(i);
                splitPages.Add(singlePagePdf);
                singlePagePdf.SaveAs($"wwwroot/pdfs/page_{i + 1}.pdf");
            }

            return Content($"Split into {splitPages.Count} pages.");
        }

        // reorder pdf pages in directory
        [HttpGet("/pdf/reorder")]
        public IActionResult ReorderPdf()
        {
            var pdf = PdfDocument.FromFile("wwwroot/pdfs/multipage.pdf");
            pdf.Pages.Reverse();
            pdf.SaveAs("wwwroot/pdfs/reordered.pdf");
            return File(System.IO.File.ReadAllBytes("wwwroot/pdfs/reordered.pdf"), "application/pdf", "Reordered.pdf");
        }

        // make pdf formatted with header and footer
        [HttpGet("/pdf/format")]
        public IActionResult AddFormatting()
        {
            var renderer = new HtmlToPdf();

            renderer.PrintOptions.HtmlHeader = new HtmlHeaderFooter()
            {
                HtmlFragment = "<span style='font-size:25px;'>Header - Page: {page}</span>",
                DrawDividerLine = true
            };

            renderer.PrintOptions.HtmlFooter = new HtmlHeaderFooter()
            {
                HtmlFragment = "<span style='font-size:25px;'>Footer - {date}</span>",
                DrawDividerLine = true
            };

            var pdf = renderer.RenderHtmlAsPdf("<h1>Document with Header/Footer</h1><p>This is sample content.</p>");

            // Fix for CS1061 and CS0234: Use ApplyWatermark method instead of WatermarkAllPages
            pdf.ApplyWatermark("<div style='color:rgba(200,0,0,0.3);font-size:48px;'>CONFIDENTIAL</div>", 50, VerticalAlignment.Middle, HorizontalAlignment.Center);

            return File(pdf.BinaryData, "application/pdf", "Formatted.pdf");
        }

        // extract text from pdf
        [HttpGet("/pdf/extract-text")]
        public IActionResult ExtractText()
        {
            var pdf = PdfDocument.FromFile("wwwroot/pdfs/sample.pdf");

            var text = pdf.ExtractAllText();
            return Content(text);
        }

        // Fix for CS1061: Replace 'AllowUserEditing' with 'AllowUserEdits' which is a valid property of PdfSecuritySettings.
        [HttpGet("/pdf/secure")]
        public IActionResult SecurePdf()
        {
            var pdfPath = "wwwroot/pdfs/sample.pdf"; // Make sure this exists
            var outputPath = "wwwroot/pdfs/secured.pdf";

            // Load existing PDF
            var pdf = PdfDocument.FromFile(pdfPath);

            // Set security options
            pdf.SecuritySettings.UserPassword = "user123";     // Required to open the PDF
            pdf.SecuritySettings.OwnerPassword = "owner456";   // Required to change permissions

            // Set specific permissions
            pdf.SecuritySettings.AllowUserCopyPasteContent = false;
            pdf.SecuritySettings.AllowUserPrinting = PdfPrintSecurity.FullPrintRights; // Fix for CS0029: Use the correct enum value

            pdf.SecuritySettings.AllowUserEdits = PdfEditSecurity.NoEdit; // Correct property to disable editing

            // Save secured version
            pdf.SaveAs(outputPath);

            return File(System.IO.File.ReadAllBytes(outputPath), "application/pdf", "Secured.pdf");
        }


    }
}
