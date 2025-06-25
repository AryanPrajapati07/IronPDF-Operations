using IronWebScraper;
using System.Collections.Generic;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace IronSofts.Scrapers
{
    public class NewsScraper : WebScraper
    {
        public List<string> Headlines { get; private set; } = new();

        public override void Init()
        {
            this.LoggingLevel = LogLevel.All;
            
            //this.Request("https://www.techmeme.com", Parse);
            this.Request("https://www.thehindu.com/news/international/", Parse);
        }

        public override void Parse(Response response)
        {
            System.IO.File.WriteAllText("scraper_debug.html", response.Html);

            //var headlineNodes = response.Css("strong > a");
            var headlineNodes = response.Css("h3.title");

            foreach (var node in headlineNodes)
            {
                var title = node.TextContent.Trim();
               
                    Headlines.Add(title);
                
            }

            Console.WriteLine($"✅ Headlines found: {Headlines.Count}");
        }
    }
}
