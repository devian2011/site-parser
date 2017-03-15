using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SiteParser.Application.Loader.Rules
{
    class RobotsRules : ILinkRule
    {
        /// <summary>
        /// List of regular expressions rules
        /// </summary>
        private LinkedList<Rule> _rules;

        /// <summary>
        /// Download and parse robots rules
        /// </summary>
        /// <param name="domain"></param>
        public RobotsRules(Uri domain)
        {
            _rules = new LinkedList<Rule>();
            //Upload robots
            var robotsUri = new Uri(domain, "/robots.txt");
            var downloadService = new PageDownloadService();
            downloadService.onContentDownload += parseRobots;
            downloadService.downloadPage(robotsUri);
        } 

        /// <summary>
        /// Parse robots.txt rules to regular expressions
        /// </summary>
        /// <param name="page"></param>
        private void parseRobots(HtmlPage page)
        {
            var disalowedRules = new HashSet<string>();
            var allowedRules = new HashSet<string>();

            string[] content = page.getBody().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            var isDisallow = new Regex(@"^Disallow: ");
            var isAllow = new Regex(@"^Allow: ");
            foreach ( string rule in content)
            {
                if (string.IsNullOrWhiteSpace(rule)) continue;

                if( isAllow.IsMatch(rule) && !allowedRules.Contains(rule))
                {
                    allowedRules.Add(rule);
                    addToRuleList( 
                            isAllow.Replace(rule,""),
                            isAllowed: true
                        );
                }
                else if( isDisallow.IsMatch(rule) && !disalowedRules.Contains(rule))
                {
                    disalowedRules.Add(rule);
                    addToRuleList(
                            isDisallow.Replace(rule, ""), 
                            isAllowed: false
                        );
                }
            }
        }

        /// <summary>
        /// Append rule to rule list
        /// Allow rules to begin and disallow rules to end
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isAllowed"></param>
        private void addToRuleList(string url, bool isAllowed)
        {
            var regex = stringToRegexExpression(url);
            if (!string.IsNullOrWhiteSpace(regex))
            {
                //Append allow rules to begin of filters
                if( isAllowed)
                {
                    _rules.AddFirst(
                            new Rule
                            {
                                rule = new Regex(@"^" + regex),
                                isAllow = isAllowed
                            }
                        );
                }
                else
                {
                    _rules.AddLast(
                            new Rule
                            {
                                rule = new Regex(@"^" + regex),
                                isAllow = isAllowed
                            }
                        );
                }
            }
        }

        /// <summary>
        /// Convert robot expression to c# regular expression
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        private string stringToRegexExpression(string rule)
        {
            return rule
                    .Trim(' ')
                    .Replace("/", "\\/")
                    .Replace("-", "\\-")
                    .Replace("?", "\\?")
                    .Replace("|", "\\|")
                    .Replace("&", "\\&")
                    .Replace(".", "\\.")
                    .Replace("*", ".*");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">Parsing url</param>
        /// <returns></returns>
        public bool allow(Uri url)
        {
            var path = url.PathAndQuery;
            foreach(Rule rule in _rules)
            {
                if (rule.isMatch(path))
                {
                    return rule.isAllow;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// Robot rule model
    /// </summary>
    internal class Rule
    {
        public Regex rule;
        public bool isAllow;


        public bool isMatch(string link)
        {
            return rule.IsMatch(link);
        }
    }

}
