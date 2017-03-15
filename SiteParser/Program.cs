using System;
using System.Collections.Generic;
using System.Net;
using SiteParser.Application.Loader;
using SiteParser.Application.Sitemap;

namespace SiteParser
{
    class Program
    {

        private static bool _closeProgram = false;

        static void Main(string[] args)
        {
            var site = new Uri(args[0]);
            var savePath = args[1].ToString();

            SiteSpiderLoader spider = new SiteSpiderLoader(site);
            SiteMapService sitemap = new SiteMapService(savePath);
            // Set events
            spider.onPageLoad += sitemap.appendPage;
            spider.onParseEnd += sitemap.saveSiteMap;
            sitemap.onSiteMapCreated += closeProgram;
            //Parse site
            spider.parse();
            while (_closeProgram == false) { }
        }

        public static void closeProgram()
        {
            _closeProgram = true;
        }
    }
}
