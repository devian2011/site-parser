using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using SiteParser.Application.Loader.Rules;
using System.Threading;
using System.Threading.Tasks;

namespace SiteParser.Application.Loader
{
    class SiteSpiderLoader
    {
        /// <summary>
        /// Max request per page
        /// Default: 3;
        /// </summary>
        private int MaxRequest = 3;

        public delegate void ContentParsing(HtmlPage page);
        public event ContentParsing onPageLoad;

        public delegate void ParsingEnd();
        public event ParsingEnd onParseEnd;

        private ConcurrentDictionary<string,int> _viewedUrls;
        private ConcurrentQueue<string> _urlCollection;
        private ConcurrentDictionary<string, int> _errorCounter;
        
        private HtmlInternalLinkFinder _linkFinder;

        private Uri _domain;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteUrl"></param>
        public SiteSpiderLoader(Uri siteUrl, List<ILinkRule> rules = null)
        {
            _domain = siteUrl;
            _urlCollection = new ConcurrentQueue<string>();
            _viewedUrls = new ConcurrentDictionary<string, int>();
            _errorCounter = new ConcurrentDictionary<string, int>();
            _linkFinder = new HtmlInternalLinkFinder(_domain, rules);
        }

        /// <summary>
        /// Parse 
        /// </summary>
        public void parse()
        {
            parseMainPage();
            parseAdditionalPages();
            onParseEnd();
        }
        
        /// <summary>
        /// Parse main page of domain
        /// </summary>
        private void parseMainPage()
        {
            var pds = new PageDownloadService();
            pds.onContentDownload += parsePage;
            pds.downloadPage(uri: _domain);
        }

        /// <summary>
        /// Parse additional pages of site at many threads
        /// </summary>
        private void parseAdditionalPages()
        {

            var tasks = new ParallelTask(5);
            tasks.setTask(() =>
            {
                var url = "";
                while (_urlCollection.TryDequeue(out url))
                {
                    if (!string.IsNullOrEmpty(url) && isNotAlreadyViewed(url))
                    {
                        try
                        {
                            var pds = new PageDownloadService();
                            pds.onContentDownload += parsePage;
                            pds.downloadPage(new Uri(url));
                        }
                        catch (System.Net.WebException connectionError)
                        {
                            requeue(url);
                        }
                    }
                }
            });
            tasks.start();
            tasks.Wait();
        }

        /// <summary>
        /// Download page headers and body html string
        /// </summary>
        /// <param name="page"></param>
        private void parsePage(HtmlPage page)
        {
            _viewedUrls.TryAdd(page.getUrl().AbsoluteUri, 1);
            appendToUrlQuery(_linkFinder.getLinks(page.getBody()));
            onPageLoad(page);
        }

        /// <summary>
        /// Get list of parsed urls at page body. And set new url to queue
        /// </summary>
        /// <param name="urls"></param>
        private void appendToUrlQuery(List<Uri> urls)
        {
            foreach( Uri url in urls)
            {
                if( isNotAlreadyViewed(url.AbsoluteUri) && !_urlCollection.Contains(url.AbsoluteUri) )
                {
                    _urlCollection.Enqueue(url.AbsoluteUri);
                }
            }
        }

        /// <summary>
        /// Check that page is not already parsed
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool isNotAlreadyViewed(string url)
        {
            return !_viewedUrls.ContainsKey(url);
        }

        /// <summary>
        /// Requeue url
        /// </summary>
        /// <param name="url"></param>
        private void requeue(string url)
        {
            int currentErrorValue = 1;
            _errorCounter.TryGetValue(url, out currentErrorValue);
            if (!_errorCounter.ContainsKey(url)) {
                _errorCounter.TryAdd(url, currentErrorValue);
            }
            if (currentErrorValue >= MaxRequest) {
                return;
            }
            //Increment error counter
            _errorCounter.TryUpdate(url, currentErrorValue + 1, currentErrorValue );
            //Drop already views list
            if (!isNotAlreadyViewed(url))
            {
                int tInt;
                _viewedUrls.TryRemove(url, out tInt);
            }
            //Set url to queue again for reparse
            _urlCollection.Enqueue(url);
        }

    }

}
