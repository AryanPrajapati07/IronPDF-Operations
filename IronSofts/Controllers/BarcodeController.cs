using Microsoft.AspNetCore.Mvc;
using IronBarCode;
using System.Drawing.Imaging;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace IronSofts.Controllers
{
    public class BarcodeController : Controller
    {
        [HttpGet("/barcode/generate")]
        public IActionResult GenerateBarcode()
        {
            var qrCode = QRCodeWriter.CreateQrCode("https://www.excelsiortechnologies.com", 300);
            qrCode.SetMargins(10);
            qrCode.ChangeBarCodeColor(System.Drawing.Color.Black);

            string path = "wwwroot/images/Qr_Code.png";
            qrCode.SaveAsPng(path);

            return File(System.IO.File.ReadAllBytes(path), "image/png", "Qr_Code.png");
        }

        [HttpGet("/barcode/qr-logo")]
        public IActionResult GenerateQRCodeWithLogo()
        {
            var qrCodeLogo = new QRCodeLogo("wwwroot/images/logo.png");
            var qrCodeWithLogo = QRCodeWriter.CreateQrCodeWithLogo("https://excelsiortechnologies.com", qrCodeLogo, 400);

            string outputPath = "wwwroot/images/qr_with_logo.png";
            qrCodeWithLogo.SaveAsPng(outputPath);

            return File(System.IO.File.ReadAllBytes(outputPath), "image/png", "qr_with_logo.png");
        }

        [HttpGet("/barcode/read")]
        public IActionResult Read()
        {
            return View(); // Loads Read.cshtml
        }

        [HttpPost("/barcode/read")]
        public IActionResult ReadBarcodeFromImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return Content("Image Not Found!");

            var filePath = $"wwwroot/images/uploaded.png";
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(stream);
            }

            var result = BarcodeReader.Read(filePath);
            if (result == null || result.Count() == 0)
                return Content("No barcode found!");

            var firstResult = result.First();

            return Content($"Barcode Text : {firstResult.Text}\nType : {firstResult.BarcodeType}");
        }

        [HttpGet("/barcode/pdf")]
        public IActionResult ExportBarcodeAsPdf()
        {
            // 1. Generate QR Code
            var qr = QRCodeWriter.CreateQrCode("https://excelsiortechnologies.com", 300);

            // 2. Convert AnyBitmap to System.Drawing.Bitmap
            var bitmap = qr.Image.ToBitmap<System.Drawing.Bitmap>();

            // 3. Save Bitmap to MemoryStream
            using var imageStream = new MemoryStream();
            bitmap.Save(imageStream, ImageFormat.Png);
            imageStream.Position = 0;

            // 4. Create PDF using PdfSharp
            var document = new PdfSharp.Pdf.PdfDocument();
            var page = document.AddPage();

            using (var gfx = XGraphics.FromPdfPage(page))
            using (var img = XImage.FromStream(imageStream))
            {
                double width = img.PixelWidth * 72.0 / img.HorizontalResolution;
                double height = img.PixelHeight * 72.0 / img.VerticalResolution;
                double x = (page.Width - width) / 2;
                double y = (page.Height - height) / 2;
                gfx.DrawImage(img, x, y, width, height);
            }

            // 5. Save PDF to stream and return
            using var pdfStream = new MemoryStream();
            document.Save(pdfStream, false);

            return File(pdfStream.ToArray(), "application/pdf", "barcode.pdf");
        }
    }
}
