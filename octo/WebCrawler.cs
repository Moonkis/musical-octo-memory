using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace octo
{
    struct CrawlerSettings
    {
        public string OutputDirectory { get; set; }
    }

    internal class WebCrawler
    {
        private CrawlerSettings m_Settings;

        public WebCrawler(CrawlerSettings settings) 
        {
            if (string.IsNullOrEmpty(settings.OutputDirectory))
            {
                _ = settings.OutputDirectory ?? throw new ArgumentNullException(nameof(settings.OutputDirectory));
                throw new ArgumentException($"{nameof(settings.OutputDirectory)} can't be empty.");
            }

            m_Settings = settings;
        }

        public void Crawl(Uri uri)
        {
            if (!Uri.IsWellFormedUriString(uri.OriginalString, UriKind.Absolute))
            {
                throw new ArgumentException($"{nameof(uri)} is incorrectly formed.");
            }

            HtmlWeb html = new HtmlWeb();
            var document = html.Load(uri);

            var localPath = GetLocalPath(uri);
            var extension = Path.GetExtension(localPath);

            if (string.IsNullOrEmpty(extension))
            {
                localPath = Path.Combine(localPath, "index.html");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(localPath));
            document.Save(localPath);
        }

        private string GetLocalPath(Uri uri)
        {
            return $"{m_Settings.OutputDirectory}/{uri.Host}{uri.AbsolutePath}";
        }
    }
}
