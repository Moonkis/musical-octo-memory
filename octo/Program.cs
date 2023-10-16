using System.Configuration;

namespace octo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var crawler = new WebCrawler(new CrawlerSettings() 
            { 
                OutputDirectory = ConfigurationManager.AppSettings["OutputDirectory"] ?? string.Empty,
                MaxDepth = int.Parse(ConfigurationManager.AppSettings["MaxDepth"] ?? "0"),
            },
            new HtmlPage());

            await crawler.AsyncCrawl(new Uri(ConfigurationManager.AppSettings["Url"] ?? string.Empty));
        }
    }
}