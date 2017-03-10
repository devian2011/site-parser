using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SiteParser
{
    class Program
    {
        static void Main(string[] args)
        {

            string domainUrl = "http://www.velosite.local:3114";
            WebClient wClient = new WebClient();
            wClient.BaseAddress = domainUrl;
            string htmlContent = wClient.DownloadString("/");
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlContent);
            foreach( HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                Console.WriteLine(node.Attributes["href"].Value);
            }
            Console.ReadKey();
        }
    }
}
