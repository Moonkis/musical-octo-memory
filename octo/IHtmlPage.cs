using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace octo
{
    internal interface IHtmlPage
    {
        public List<PageInformation> GetChildPages();
        public List<PageInformation> GetStaticResources();
        public string Load(Uri uri);
        public bool Save(string filePath);
    }
}
