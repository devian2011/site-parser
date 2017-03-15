using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using SiteParser.Application.Loader.Rules;

namespace SiteParser.Application.Loader
{

    class HtmlInternalLinkFinder
    {
        protected Uri _domain;
        protected List<ILinkRule> _rules;


        public HtmlInternalLinkFinder(Uri domain, List<ILinkRule> rule = null)
        {
            _domain = domain;
            _rules = rule == null ? new List<ILinkRule>() : rule;
        }

        public List<Uri> getLinks(string htmlContent)
        {
            var urls = new List<Uri>();
            HtmlDocument hDoc = new HtmlDocument();
            hDoc.LoadHtml(htmlContent);
            var nodes = hDoc.DocumentNode.SelectNodes("//a[@href]");
            foreach (HtmlNode node in nodes)
            {
                var link = node.Attributes["href"].Value;

                //Check that link is a link to source and not # or javascript::void(0) and etc
                if (!isLink(link)) continue;

                //Replace double // to main request http scheme
                link = Regex.Replace(link, @"^\/\/", _domain.Scheme + "://");
                var url = isLinkAbsolute(link)
                    ? new Uri(link)
                    : new Uri(_domain, link);
                //Check that link is allowed to add
                if( isLinkAllowed(url) )
                {
                    urls.Add(url);
                }
            }
            return urls;
        }

        /// <summary>
        /// Check that link is a link to source and not # or javascript::void(0) and etc
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public bool isLink(string link)
        {
            var isLink = new Regex(@"^(http:|https:|\/{1,2})");
            return isLink.IsMatch(link);
        }

        /// <summary>
        /// Walk on all filters and check that link is allow to parse
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public bool isLinkAllowed(Uri link)
        {
            foreach(var rule in _rules)
            {
                if (!rule.allow(link))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check that link is absolute
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public bool isLinkAbsolute(string link)
        {
            var isAbsoluteLink = new Regex(@"^(http:|https:|\/\/)");
            return isAbsoluteLink.IsMatch(link);
        }

    }
}
