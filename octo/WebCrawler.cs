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

        public event EventHandler<(int TotalProcessed, int TotalQueued)> ProgressUpdated;

        private CrawlerSettings m_Settings;
        private HashSet<LinkInformation> m_Processed;
        private Queue<LinkInformation> m_QueuedPages;
        private IHtmlPage m_HtmlPage;
        private IResourceDownloader m_ResourceDownloader;

        public WebCrawler(CrawlerSettings settings, IHtmlPage htmlPage, IResourceDownloader resourceDownloader) 
        {
            if (string.IsNullOrEmpty(settings.OutputDirectory))
            {
                _ = settings.OutputDirectory ?? throw new ArgumentNullException(nameof(settings.OutputDirectory));
                throw new ArgumentException($"{nameof(settings.OutputDirectory)} can't be empty.");
            }

            m_Settings = settings;
            m_Processed = new HashSet<LinkInformation>(linkInformationComparer);
            m_QueuedPages = new Queue<LinkInformation>();
            m_HtmlPage = htmlPage;
            m_ResourceDownloader = resourceDownloader;
        }

        public async Task AsyncCrawl(Uri uri)
        {
            if (!Uri.IsWellFormedUriString(uri.OriginalString, UriKind.Absolute))
            {
                throw new ArgumentException($"{nameof(uri)} is incorrectly formed.");
            }

            m_QueuedPages.Enqueue(new LinkInformation(uri.OriginalString, 0));

            while (m_QueuedPages.Any())
            {
                if (m_QueuedPages.TryDequeue(out var pageInfo) && (m_Settings.MaxDepth != -1 && pageInfo.Depth > m_Settings.MaxDepth) && !m_Processed.Contains(pageInfo))
                {
                    continue;
                }

                m_HtmlPage.Load(pageInfo.Uri);
                m_Processed.Add(pageInfo);
                m_HtmlPage.Save(GetLocalPath(pageInfo.Uri));

                var resources = m_HtmlPage.GetStaticResources(pageInfo);
                var pages = m_HtmlPage.GetChildPages(pageInfo);

                await ProcessResources(resources);

                foreach (var page in pages)
                {
                    if (m_Processed.Contains(page) || m_QueuedPages.Contains(page, linkInformationComparer))
                    {
                        continue;
                    }

                    m_QueuedPages.Enqueue(page);
                }

                ProgressUpdated?.Invoke(this, (m_Processed.Count, m_QueuedPages.Count));
            }
        }

        private string GetLocalPath(Uri uri) => 
            $"{m_Settings.OutputDirectory}/{uri.Host}{uri.AbsolutePath}";

        private async Task ProcessResources(IEnumerable<LinkInformation> resources)
        {
            foreach (var chunk in resources.Chunk(m_ResourceDownloader.MaxConcurrent))
            {
                var filtered = new List<LinkInformation>(chunk.Length);
                foreach (var resource in chunk)
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
        }
    }
}