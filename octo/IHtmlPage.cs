using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace octo
{
    internal interface IHtmlPage
    {
        public List<LinkInformation> GetChildPages(LinkInformation info);
        public List<LinkInformation> GetStaticResources(LinkInformation info);
        public bool Load(Uri uri);
        public bool Save(string filePath);
    }
}
