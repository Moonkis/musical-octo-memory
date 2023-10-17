using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace octo
{
    internal class PageScraper : IPageScraper
    {
        private readonly object m_Lock = new();
        private ParallelOptions m_ParallelOptions;

        public PageScraper(ParallelOptions options) 
        {
            m_ParallelOptions = options;
        }

        public PageResult VisitAllPages(IEnumerable<(LinkInformation info, string outputPath)> pages)
        {
            var resultResources = new List<LinkInformation>();
            var resultChildPages = new List<LinkInformation>();

            var result = Parallel.ForEach(pages, m_ParallelOptions, page =>
            {
                IHtmlPage pageLoader = new HtmlPage();
                pageLoader.Load(page.info.Uri);
                var resources = pageLoader.GetStaticResources(page.info);
                var childPages = pageLoader.GetChildPages(page.info);

                pageLoader.Save(page.outputPath);
                lock(m_Lock)
                {
                    resultResources.AddRange(resources);
                    resultChildPages.AddRange(childPages);
                }
            });

            return new PageResult(resultResources, resultChildPages);
        }
    }
}
