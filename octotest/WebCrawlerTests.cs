using Moq;
using octo;

namespace octotest
{
    [TestClass]
    public class WebCrawlerTests
    {
        private CrawlerSettings m_Settings;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Mock<IPageScraper> m_Scraper;
        private Mock<IResourceDownloader> m_ResourceDownloader;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [TestInitialize]
        public void Setup()
        {
            m_Settings = new CrawlerSettings()
            {
                MaxDepth = -1,
                OutputDirectory = "sometest"
            };

            m_Scraper = new Mock<IPageScraper>();
            m_ResourceDownloader = new Mock<IResourceDownloader>();
        }

        [TestMethod]
        public async Task PageLinksAreNotVisitedMoreThanOnceOK()
        {
            bool noDuplicates = true;

            m_Scraper.Setup(x => x.VisitAllPages(It.IsAny<IEnumerable<(LinkInformation info, string path)>>()))
                .Callback<IEnumerable<(LinkInformation info, string path)>>((pages) =>
                {
                    noDuplicates &= pages.Count() == pages.Distinct().Count();
                })
                .Returns(new PageResult(new List<LinkInformation>(), GetChildPages()));



            var crawler = new WebCrawler(m_Settings, m_Scraper.Object, m_ResourceDownloader.Object);

            await crawler.AsyncCrawl(new Uri("https://www.dummylink/"));

            Assert.IsTrue(noDuplicates);
        }

        [TestMethod]
        public async Task ResourceLinksAreNotDownloadedMoreThanOnceOK()
        {
            bool noDuplicates = true;
            m_Scraper.Setup(x => x.VisitAllPages(It.IsAny<IEnumerable<(LinkInformation info, string path)>>()))
                .Returns(new PageResult(GetResources(), GetChildPages()));

            m_ResourceDownloader.Setup(x => x.DownloadAll(It.IsAny<IEnumerable<(LinkInformation info, string path)>>()))
                .Callback<IEnumerable<(LinkInformation info, string path)>>((pages) =>
                {
                    noDuplicates &= pages.Count() == pages.Distinct().Count();
                });

            var crawler = new WebCrawler(m_Settings, m_Scraper.Object, m_ResourceDownloader.Object);

            await crawler.AsyncCrawl(new Uri("https://www.dummylink/"));

            Assert.IsTrue(noDuplicates);
        }

        [TestMethod]
        public void WebCrawlerThrowsArgumentExceptionWhenOutputDirectoryIsEmpty()
        {
            var settings = new CrawlerSettings()
            {
                OutputDirectory = string.Empty,
                MaxDepth = -1
            };

            Assert.ThrowsException<ArgumentException>(() => new WebCrawler(settings, m_Scraper.Object, m_ResourceDownloader.Object));
        }

        [TestMethod]
        public void WebCrawlerThrowsArgumentNullExceptionWhenOutputDirectoryIsNull()
        {
            var settings = new CrawlerSettings()
            {
                OutputDirectory = null,
                MaxDepth = -1
            };

            Assert.ThrowsException<ArgumentNullException>(() => new WebCrawler(settings, m_Scraper.Object, m_ResourceDownloader.Object));
        }

        private List<LinkInformation> GetChildPages() => new List<LinkInformation>()
            {
                new LinkInformation("https://dummylink/a/index.html", 0),
                new LinkInformation("https://dummylink/c/index.html", 0),
                new LinkInformation("https://dummylink/c/index.html", 0),
                new LinkInformation("https://dummylink/c/index.html", 0),
                new LinkInformation("https://dummylink/d/index.html", 0),
                new LinkInformation("https://dummylink/d/index.html", 0),
                new LinkInformation("https://dummylink/d/index.html", 0),
                new LinkInformation("https://dummylink/e/index.html", 0),
                new LinkInformation("https://dummylink/e/index.html", 0),
                new LinkInformation("https://dummylink/e/index.html", 0),
                new LinkInformation("https://dummylink/f/index.html", 0),
            };

        private List<LinkInformation> GetResources() => new List<LinkInformation>()
            {
                new LinkInformation("https://dummylink/a/a.js", 0),
                new LinkInformation("https://dummylink/c/a.js", 0),
                new LinkInformation("https://dummylink/c/b.jpg", 0),
                new LinkInformation("https://dummylink/c/b.jpg", 0),
                new LinkInformation("https://dummylink/d/c.css", 0),
                new LinkInformation("https://dummylink/d/c.css", 0),
            };
    }
}