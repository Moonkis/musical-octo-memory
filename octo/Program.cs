using System.Configuration;

namespace octo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var crawler = new WebCrawler(new CrawlerSettings() 
            { 
                OutputDirectory = ConfigurationManager.AppSettings["OutputDirectory"] ?? string.Empty
            });

            crawler.Crawl(new Uri(ConfigurationManager.AppSettings["Url"] ?? string.Empty));
        }
    }
}