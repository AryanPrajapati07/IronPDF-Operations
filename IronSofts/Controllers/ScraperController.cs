using IronSofts.Scrapers;
using Microsoft.AspNetCore.Mvc;

namespace IronSofts.Controllers
{
    public class ScraperController : Controller
    {
        [HttpGet("/scraper/news")]
        public IActionResult ScrapeNews()
        {
            var scraper = new NewsScraper();
            scraper.Start(); // asynchronous, blocks main thread
            System.Threading.Thread.Sleep(5000); // crude wait

            if (scraper.Headlines.Count == 0)
            {
                ViewData["Error"] = "No headlines found. Try again or inspect HTML structure.";
            }

            return View(scraper.Headlines);
        }
    }
}
