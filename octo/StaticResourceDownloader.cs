using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace octo
{
    internal class StaticResourceDownloader : IResourceDownloader
    {
        private static readonly int DefaultMaxConcurrent = 5;
        private static readonly HttpClient m_HttpClient = new HttpClient();
        private ParallelOptions m_ParallelOptions;

        public int MaxConcurrent => m_ParallelOptions?.MaxDegreeOfParallelism ?? DefaultMaxConcurrent;

        public StaticResourceDownloader(ParallelOptions options)
        {
            m_ParallelOptions = options;
        }

        public async Task DownloadAll(IEnumerable<(LinkInformation info, string outputPath)> pages)
        {
            await Parallel.ForEachAsync(pages, m_ParallelOptions, async (page, cancellationToken) =>
            {
                using HttpResponseMessage response = await m_HttpClient.GetAsync(page.info.Uri, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(page.outputPath));
                    using FileStream fileStream = new FileStream(page.outputPath, FileMode.Create);
                    await response.Content.CopyToAsync(fileStream);
                }
            });
        }
    }
}
