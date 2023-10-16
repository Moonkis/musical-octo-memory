using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace octo
{
    internal class HtmlPage : IHtmlPage
    {
        private HtmlWeb m_HtmlWeb;
        private HtmlDocument m_HtmlDocument;

        public HtmlPage()
        {
            m_HtmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };

            m_HtmlDocument = new HtmlDocument();
        }

        public List<PageInformation> GetChildPages(PageInformation info)
        {
            var pages = new List<PageInformation>();
            foreach (var link in m_HtmlDocument.DocumentNode.SelectNodes("//a[@href]"))
            {
                var href = link.Attributes["href"].Value;
                if (string.IsNullOrEmpty(href))
                {
                    continue;
                }

                pages.Add(new PageInformation(new Uri(info.Uri, href).AbsoluteUri, info.Depth + 1));
            }

            return pages;
        }

        public List<PageInformation> GetStaticResources(PageInformation info)
        {
            var resources = new List<PageInformation>();

            foreach (var link in m_HtmlDocument.DocumentNode.SelectNodes("//script[@src] | //img[@src] | //link[@href]"))
            {
                var val = link.Attributes.FirstOrDefault(x => x.Name == "src")?.Value ?? link.Attributes["href"].Value;

                if (string.IsNullOrEmpty(val))
                {
                    continue;
                }

                resources.Add(new PageInformation(new Uri(info.Uri, val).AbsoluteUri, info.Depth + 1));
            }

            return resources;
        }

        public bool Load(Uri uri)
        {
            try
            {
                m_HtmlDocument = m_HtmlWeb.Load(uri);
            }
            catch(HtmlWebException)
            {
                return false;
            }
            catch(ArgumentNullException)
            {
                return false;
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        public bool Save(string filePath)
        {
            if (m_HtmlDocument == null)
            {
                return false;
            }

            try
            {
                m_HtmlDocument.Save(filePath);
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }
    }
}
