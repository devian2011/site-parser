using System;
using System.Net;
using System.Threading.Tasks;

namespace SiteParser.Application.Loader
{
    class PageDownloadService
    {

        public delegate void ContentParsing(HtmlPage page);
        public event ContentParsing onContentDownload;

        private Uri _uri;

        public void downloadPage(Uri uri)
        {
            var webClient = new WebClient();
            var result = webClient.DownloadString(uri);
            var htmlPage = new HtmlPage(uri.AbsoluteUri, new HtmlHeader(webClient.ResponseHeaders), result);
            onContentDownload(htmlPage);
        }


        public void downloadPageAsync(Uri uri)
        {
            _uri = uri;
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
            webClient.DownloadStringAsync(_uri);
        }

        private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if( e.Cancelled != false && e.Error != null)
            {
                var wClient = (WebClient)sender;
                var htmlPage = new HtmlPage(_uri.AbsoluteUri, new HtmlHeader(wClient.ResponseHeaders), e.Result);
                onContentDownload(htmlPage);
            }

        }
    }
}
