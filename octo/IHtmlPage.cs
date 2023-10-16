using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace octo
{
    internal interface IHtmlPage
    {
        public List<PageInformation> GetChildPages(PageInformation info);
        public List<PageInformation> GetStaticResources(PageInformation info);
        public bool Load(Uri uri);
        public bool Save(string filePath);
    }
}
