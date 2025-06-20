using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using IronPdf.Editing;
using IronPdf.Security;
using IronSofts.Services;
using Microsoft.AspNetCore.Identity;
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

            var pdf = renderer.RenderHtmlAsPdf("<h1>Document with Header/Footer and Wtaermark.</h1><p>This is sample content.</p>");

           
            pdf.ApplyWatermark("<div style='color:rgba(200,0,0,0.8);font-size:48px;'>WATERMARK</div>", 50, VerticalAlignment.Middle, HorizontalAlignment.Center);

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

        
        [HttpGet("/pdf/secure")]
        public IActionResult SecurePdf()
        {
            var pdfPath = "wwwroot/pdfs/sample.pdf"; 
            var outputPath = "wwwroot/pdfs/secured.pdf";

            // Load existing PDF
            var pdf = PdfDocument.FromFile(pdfPath);

            // Set security options
            pdf.SecuritySettings.UserPassword = "user123";     
            pdf.SecuritySettings.OwnerPassword = "owner456";   

            // Set specific permissions
            pdf.SecuritySettings.AllowUserCopyPasteContent = false;
            pdf.SecuritySettings.AllowUserPrinting = PdfPrintSecurity.FullPrintRights; 

            pdf.SecuritySettings.AllowUserEdits = PdfEditSecurity.NoEdit; 

            // Save secured version
            pdf.SaveAs(outputPath);

            return File(System.IO.File.ReadAllBytes(outputPath), "application/pdf", "Secured.pdf");
        }

        // Update the problematic code block in the AddContentToPdf method
        [HttpGet("/pdf/add-content")]
        public IActionResult AddContentToPdf()
        {
            // 1. Generate QR Code Image
            //var qrGenerator = new QRCodeGenerator();
            //var qrData = qrGenerator.CreateQrCode("https://excelsiortechnologies.com", QRCodeGenerator.ECCLevel.Q);
            //var qrCode = new QRCode(qrData);

            //var qrImagePath = "wwwroot/images/temp_qr.png";
            //using (var qrBitmap = qrCode.GetGraphic(20) as Bitmap) // Cast to Bitmap explicitly
            //{
            //    if (qrBitmap != null) // Ensure the cast was successful
            //    {
            //        qrBitmap.Save(qrImagePath, ImageFormat.Png); // Save the image
            //    }
            //}

            // 2. Build the HTML
            var html = @"
            <html>
            <head>
                <style>
                    body {
                        font-family: Arial;
                        padding: 40px;
                    }
                    .text {
                        font-size: 24px;
                        color: red;
                        font-weight: bold;
                    }
                    img {
                        margin-top: 20px;
                    }
                </style>
            </head>
            <body>
                <div class='text'>CONFIDENTIAL</div>
                <p>This is your PDF with embedded text, logo, and QR.</p>
                <img src='file:./wwwroot/images/logo.png' width='150' />
                <br />
                <img src='file:./wwwroot/images/temp_qr.png' width='100' />
            </body>
            </html>";

            // 3. Render HTML to PDF
            var renderer = new HtmlToPdf();
            var pdf = renderer.RenderHtmlAsPdf(html);

            // 4. Return as file
            return File(pdf.BinaryData, "application/pdf", "WithContent.pdf");
        }

    }
}
