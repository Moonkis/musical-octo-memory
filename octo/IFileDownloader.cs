using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace octo
{
    internal interface IResourceDownloader
    {
        public Task DownloadAll(IEnumerable<(LinkInformation info, string outputPath)> pages);
        public int MaxConcurrent { get; }
    }
}
