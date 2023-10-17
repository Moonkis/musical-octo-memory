using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace octo
{
    internal class WebCrawler
    {
        private static readonly LinkInformationComparer linkInformationComparer = new LinkInformationComparer();

        public event EventHandler<(int TotalProcessed, int TotalQueued)>? ProgressUpdated;

        private CrawlerSettings m_Settings;
        private HashSet<LinkInformation> m_Processed;
        private HashSet<LinkInformation> m_QueuedPages;
        private IPageScraper m_PageScraper;
        private IResourceDownloader m_ResourceDownloader;

        public WebCrawler(CrawlerSettings settings, IPageScraper pageScraper, IResourceDownloader resourceDownloader) 
        {
            if (string.IsNullOrEmpty(settings.OutputDirectory))
            {
                _ = settings.OutputDirectory ?? throw new ArgumentNullException(nameof(settings.OutputDirectory));
                throw new ArgumentException($"{nameof(settings.OutputDirectory)} can't be empty.");
            }

            m_Settings = settings;
            m_Processed = new HashSet<LinkInformation>(linkInformationComparer);
            m_QueuedPages = new HashSet<LinkInformation>(linkInformationComparer);
            m_PageScraper = pageScraper;
            m_ResourceDownloader = resourceDownloader;
        }

        public async Task AsyncCrawl(Uri uri)
        {
            if (!Uri.IsWellFormedUriString(uri.OriginalString, UriKind.Absolute))
            {
                throw new ArgumentException($"{nameof(uri)} is incorrectly formed.");
            }

            m_QueuedPages.Add(new LinkInformation(uri.OriginalString, 0));

            while (m_QueuedPages.Any())
            {
                var result = ProcessPages();
                await ProcessResources(result.Resources);
                ProgressUpdated?.Invoke(this, (m_Processed.Count, m_QueuedPages.Count));
            }
        }

        private string GetLocalPath(Uri uri) => 
            $"{m_Settings.OutputDirectory}/{uri.Host}{uri.AbsolutePath}";

        private async Task ProcessResources(IEnumerable<LinkInformation> resources)
        {
            var filtered = new List<LinkInformation>();
            foreach(var resource in resources)
            {
                if (m_Processed.Contains(resource))
                {
                    continue;
                }

                m_Processed.Add(resource);
                filtered.Add(resource);
            }

            await m_ResourceDownloader.DownloadAll(filtered.Select(x => (x, GetLocalPath(x.Uri))));
        }

        private PageResult ProcessPages()
        {
            m_Processed.UnionWith(m_QueuedPages);
            var result = m_PageScraper.VisitAllPages(m_QueuedPages.Select(x => (x, GetLocalPath(x.Uri))));

            m_QueuedPages.Clear();
            foreach (var child in result.ChildPages)
            {
                if (m_Processed.Contains(child) || 
                    m_QueuedPages.Contains(child) || 
                    (m_Settings.MaxDepth != -1 && child.Depth > m_Settings.MaxDepth))
                {
                    continue;
                }

                m_QueuedPages.Add(child);
            }

            return result;
        }
    }
}