using System;
using System.Net;

namespace SiteParser.Application.Loader
{

    class HtmlHeader
    {
        protected WebHeaderCollection _headers;

        public HtmlHeader(WebHeaderCollection headers)
        {
            _headers = headers;
        }

        public string LastModified()
        {
            return _headers[HttpResponseHeader.LastModified];
        }

    }

    class HtmlPage
    {
        protected Uri _url;
        protected HtmlHeader _header;
        protected string _htmlString;

        public HtmlPage(string pageUrl,HtmlHeader header, string html)
        {
            _url = new Uri( pageUrl );
            _header = header;
            _htmlString = html;
        }

        public Uri getUrl()
        {
            return _url;
        }

        public HtmlHeader getHeader()
        {
            return _header;
        }

        public string getBody()
        {
            return _htmlString;
        }

    }
}
