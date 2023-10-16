using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace octo
{
    struct CrawlerSettings
    {
        public string OutputDirectory { get; set; }
        public int MaxDepth { get; set; }
    }

    struct PageInformation : IEqualityComparer<PageInformation>
    {
        public Uri Uri { get; }

        public int Depth { get; }

        public PageInformation(string url, int depth)
        {
            Uri = new Uri(url);
            Depth = depth;
        }

        public bool Equals(PageInformation x, PageInformation y)
        {
            return StringComparer
                .InvariantCultureIgnoreCase
                .Equals(x.Uri.OriginalString, y.Uri.OriginalString);
        }

        public int GetHashCode(PageInformation pageInformation)
        {
            return StringComparer
                .InvariantCultureIgnoreCase
                .GetHashCode(pageInformation.Uri.OriginalString);
        }
    }

    internal class WebCrawler
    {
        private CrawlerSettings m_Settings;

        private ConcurrentHashSet<PageInformation> m_Processed;
        private ConcurrentQueue<PageInformation> m_QueuedPages;

        public WebCrawler(CrawlerSettings settings) 
        {
            if (string.IsNullOrEmpty(settings.OutputDirectory))
            {
                _ = settings.OutputDirectory ?? throw new ArgumentNullException(nameof(settings.OutputDirectory));
                throw new ArgumentException($"{nameof(settings.OutputDirectory)} can't be empty.");
            }

            m_Settings = settings;
            m_Processed = new ConcurrentHashSet<PageInformation>();
            m_QueuedPages = new ConcurrentQueue<PageInformation>();
        }

        public void Crawl(Uri uri)
        {
            if (!Uri.IsWellFormedUriString(uri.OriginalString, UriKind.Absolute))
            {
                throw new ArgumentException($"{nameof(uri)} is incorrectly formed.");
            }

            HtmlWeb html = new HtmlWeb();
            m_QueuedPages.Enqueue(new PageInformation(uri.OriginalString, 0));
            
            while(!m_QueuedPages.IsEmpty)
            {
                if (m_QueuedPages.TryDequeue(out var pageInfo) && pageInfo.Depth > m_Settings.MaxDepth && !m_Processed.Contains(pageInfo))
                {
                    continue;
                }

                var document = html.Load(pageInfo.Uri);
                m_Processed.Add(pageInfo);

                var localPath = GetLocalPath(pageInfo.Uri);
                var extension = Path.GetExtension(localPath);

                if (string.IsNullOrEmpty(extension))
                {
                    localPath = Path.Combine(localPath, "index.html");
                }

                Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                document.Save(localPath);

                var pages = GetChildPages(document, pageInfo);

                foreach (var page in pages)
                {
                    if (m_Processed.Contains(page) || m_QueuedPages.Contains(page))
                    {
                        continue;
                    }

                    m_QueuedPages.Enqueue(page);
                }
            }
        }

        private string GetLocalPath(Uri uri)
        {
            return $"{m_Settings.OutputDirectory}/{uri.Host}{uri.AbsolutePath}";
        }

        private List<PageInformation> GetChildPages(HtmlDocument document, PageInformation currentPage)
        {
            var pages = new List<PageInformation>();
            foreach (var link in document.DocumentNode.SelectNodes("//a[@href]"))
            {
                var href = link.Attributes["href"].Value;
                if (string.IsNullOrEmpty(href))
                {
                    continue;
                }

                pages.Add(new PageInformation(new Uri(currentPage.Uri, href).AbsoluteUri, currentPage.Depth + 1));
            }

            return pages;
        }
    }
}
