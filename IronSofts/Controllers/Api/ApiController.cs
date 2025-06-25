using IronSofts.Scrapers;
using Microsoft.AspNetCore.Mvc;

namespace IronSofts.Controllers.Api
{
    public class ApiController : Controller
    {
        
        [HttpGet("pdf/api-generated")]
        public IActionResult GeneratePdf()
        {
            var html = "<h1>Hello from API PDF!</h1><p>This is generated dynamically.</p>";
            var renderer = new HtmlToPdf();
            var pdf = renderer.RenderHtmlAsPdf(html);

            return File(pdf.BinaryData, "application/pdf", "api-generated.pdf");
        }

        [HttpGet("news/latest")]
        public IActionResult GetLatestHeadlines()
        {
            var scraper = new NewsScraper();
            scraper.Start();

            Thread.Sleep(5000); // wait to let scrape complete

            if (scraper.Headlines.Count == 0)
                return NotFound("No headlines found");

            return Ok(scraper.Headlines);
        }
    }
}
