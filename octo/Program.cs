using System.Configuration;

namespace octo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var crawler = new WebCrawler(new CrawlerSettings() 
            { 
                OutputDirectory = ConfigurationManager.AppSettings["OutputDirectory"] ?? string.Empty,
                MaxDepth = int.Parse(ConfigurationManager.AppSettings["MaxDepth"] ?? "0"),
            },
            new HtmlPage());

            crawler.Crawl(new Uri(ConfigurationManager.AppSettings["Url"] ?? string.Empty));
        }
    }
}