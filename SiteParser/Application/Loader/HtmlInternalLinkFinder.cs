using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace SiteParser.Application.Loader
{

    class HtmlInternalLinkFinder
    {
        protected Uri _baseUrl;
        protected SelfSiteLinkGetter _internalLinkFilter;

        public HtmlInternalLinkFinder(Uri domainUrl)
        {
            _baseUrl = domainUrl;
            _internalLinkFilter = new SelfSiteLinkGetter(domainUrl);
        }

        public List<Uri> getLinks(string htmlContent)
        {
            var urls = new List<Uri>();
            HtmlDocument hDoc = new HtmlDocument();
            hDoc.LoadHtml(htmlContent);
            var nodes = hDoc.DocumentNode.SelectNodes("//a[@href]");
            foreach (HtmlNode node in nodes)
            {
                var link = _internalLinkFilter.getter(node.Attributes["href"].Value);
                if (link.isLink && link.isInternal)
                {
                    urls.Add(link.url);
                }
            }

            return urls;
        }

    }

    /// <summary>
    /// Link data object
    /// </summary>
    class Link
    {
        public bool isLink;
        public bool isInternal;
        public Uri url;
    }

    class SelfSiteLinkGetter
    {
        protected Uri _domain;

        public SelfSiteLinkGetter(Uri domainName)
        {
            _domain = domainName;
        }


        public Link getter(string url)
        {
            var isLink = new Regex(@"^(http:|https:|\/{1,2})");
            //Check url is not javascript:void(0), # or another specials tags
            if (isLink.IsMatch(url))
            {
                var isAbsoluteLink = new Regex(@"^(http:|https:|\/\/)");
                if (isAbsoluteLink.IsMatch(url))
                {
                    url = Regex.Replace(url, @"^\/\/", _domain.Scheme + "://");
                    var isInternalLink = new Regex(@"" + _domain.Host + "\\/");
                    if (isInternalLink.IsMatch(url))
                    {
                        return new Link { isLink = true, isInternal = true, url = new Uri(url) };
                    }
                    else
                    {
                        return new Link { isLink = true, isInternal = false, url = new Uri(url) };
                    }
                }
                else
                {
                    return new Link { isLink = true, isInternal = true, url = new Uri(_domain, url) };
                }

            }

            return new Link { isLink = false };
        }
    }
}
