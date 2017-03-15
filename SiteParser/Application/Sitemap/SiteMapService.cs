using System;
using System.IO;
using SiteParser.Application.Loader;
using System.Xml;

namespace SiteParser.Application.Sitemap
{
    class SiteMapService
    {

        public delegate void SiteMapCreated();
        public event SiteMapCreated onSiteMapCreated;
        
        protected SiteMapXml _xmlDocument;
        private string _savePath;

        public SiteMapService(string savePath)
        {
            _xmlDocument = new SiteMapXml(savePath);
        }

        public void appendPage(HtmlPage page)
        {
            Console.WriteLine(page.getUrl().AbsoluteUri);
            var row = RowBuilder.build(page);
            _xmlDocument.appendRow(row);
        }

        public void saveSiteMap()
        {
            _xmlDocument.save();
            onSiteMapCreated();
        }

    }

    class SiteMapXml
    {

        protected XmlWriter _xmlDocument;

        public SiteMapXml(string pathForSave)
        {
            _xmlDocument = XmlWriter.Create(
                new FileStream(pathForSave, FileMode.Create),
                new XmlWriterSettings {
                    Encoding = System.Text.Encoding.UTF8,
                    WriteEndDocumentOnClose = true,
                    Indent = true
                } 
            );
            writeFileBegin();
        }

        public void appendRow(SiteMapRow row)
        {
            _xmlDocument.WriteStartElement("url");
            _xmlDocument.WriteElementString("loc", row.location);
            _xmlDocument.WriteElementString("lastmod", row.lastmod);
            _xmlDocument.WriteElementString("changefreq", row.chanagefreq);
            _xmlDocument.WriteElementString("priority", row.priority);
            _xmlDocument.WriteEndElement();
        }

        public void save()
        {
            _xmlDocument.Close();
        }

        private void writeFileBegin()
        {
            _xmlDocument.WriteStartDocument();
            _xmlDocument.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
        }

    }

    class RowBuilder
    {
        public static SiteMapRow build(HtmlPage page)
        {
            return new SiteMapRow
            {
                location = page.getUrl().AbsoluteUri,
                lastmod = page.getHeader().LastModified()
            };
        }
    }

}
