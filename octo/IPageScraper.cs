using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace octo
{
    struct PageResult
    {
        public PageResult(List<LinkInformation> resources, List<LinkInformation> childPages)
        {
            Resources = resources;
            ChildPages = childPages;
        }

        public List<LinkInformation> Resources { get; }
        public List<LinkInformation> ChildPages { get; }
    }

    internal interface IPageScraper
    {
        public PageResult VisitAllPages(IEnumerable<(LinkInformation info, string outputPath)> pages);
    }
}
