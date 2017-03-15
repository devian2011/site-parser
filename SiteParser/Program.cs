using System;
using System.Collections.Generic;
using SiteParser.Application.Loader;
using SiteParser.Application.Sitemap;
using SiteParser.Application.Loader.Rules;

namespace SiteParser
{
    class Program
    {
        private static bool _closeProgram = false;

        static void Main(string[] args)
        {
#if DEBUG
            var site = new Uri("https://habrahabr.ru");
            var savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\sitemap.xml";
#else
            var site = new Uri(args[0]);
            var savePath = args[1].ToString();
#endif

            List<ILinkRule> rules = new List<ILinkRule>();
            rules.Add(new SelfLinkRule(site));
            rules.Add(new NoHashesRule());
            rules.Add(new RobotsRules(site));
            //Init sitemap
            SiteSpiderLoader spider = new SiteSpiderLoader(site, rules);
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
