using System.Configuration;
using System.Diagnostics;

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
            new HtmlPage(),
            new StaticResourceDownloader(new ParallelOptions() { MaxDegreeOfParallelism = 5, CancellationToken = new CancellationToken() }));

            var time = new Stopwatch();
            time.Start();

            crawler.ProgressUpdated += (sender, progress) =>
            {
                if (sender is null)
                {
                    throw new ArgumentNullException(nameof(sender));
                }

                var delta = time.Elapsed.Ticks / progress.TotalProcessed;
                var minutes = new TimeSpan(delta * progress.TotalQueued).ToString(@"hh\:mm\:ss");

                Console.WriteLine($"{progress.TotalProcessed}/{(progress.TotalProcessed + progress.TotalQueued)}. Estimated time left: {minutes}");
            };

            await crawler.AsyncCrawl(new Uri(ConfigurationManager.AppSettings["Url"] ?? string.Empty));
            time.Stop();
        }
    }
}