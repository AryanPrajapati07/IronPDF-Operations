using Microsoft.AspNetCore.Mvc;
using IronPdf;
using IronOcr;


namespace IronSofts.Controllers
{
    public class OcrController : Controller
    {
        [HttpGet("/ocr/image")]
        public IActionResult ReadImageText()
        {
            var ocr = new IronTesseract();
            using (var input = new OcrInput("wwwroot/images/scanned-invoice.png"))
            {
                var result = ocr.Read(input);
                return Content($"📝 OCR Result:\n\n{result.Text}");
            }
        }

        [HttpGet("/ocr/pdf")]
        public IActionResult ReadPdfText()
        {
            var ocr = new IronTesseract();
            using (var input = new OcrInput("wwwroot/pdfs/file1.pdf"))
            {
                var result = ocr.Read(input);
                return Content($"📄 PDF OCR Result:\n\n{result.Text}");
            }
        }

        [HttpGet("/ocr/multi-lang")]
        public IActionResult ReadMultiLanguage()
        {
            var ocr = new IronTesseract();

           
            ocr.Language = OcrLanguage.Hindi | OcrLanguage.English; 

            using (var input = new OcrInput("wwwroot/images/multilanguage.png"))
            {
                var result = ocr.Read(input);
                return Content($"🌍 OCR Multi-language Result:\n\n{result.Text}");
            }
        }

        [HttpGet("/ocr/table")]
        public IActionResult ExtractTable()
        {
            var ocr = new IronTesseract();
            ocr.Configuration.EngineMode = TesseractEngineMode.TesseractAndLstm;

            using (var input = new OcrInput("wwwroot/images/table.png"))
            {
                input.DeNoise();     // Clean blurry scan
                input.EnhanceResolution(); // Increase resolution
                var result = ocr.Read(input);
                return Content($"📊 Extracted Table:\n\n{result.Text}");
            }
        }

        [HttpGet("/ocr/handwriting")]
        public IActionResult ReadHandwriting()
        {
            var ocr = new IronTesseract();

            // Optional: Increase OCR quality
            ocr.Configuration.TesseractVersion = TesseractVersion.Tesseract5;
            ocr.Configuration.ReadBarCodes = false;
            ocr.Configuration.RenderSearchablePdfsAndHocr = false;

            using (var input = new OcrInput("wwwroot/images/handwritten.png"))
            {
                input.DeNoise();            // Remove noise
                input.Deskew();             // Straighten lines
                //input.EnhanceContrast();    // Make darks darker
                input.EnhanceResolution();  // Upscale if needed
                input.ToGrayScale();          // Reduce background colors

                var result = ocr.Read(input);
                return Content(result.Text);
            }

        }

    }
}
