using System;
using System.Text.RegularExpressions;

namespace SiteParser.Application.Loader.Rules
{
    class SelfLinkRule : ILinkRule
    {

        /// <summary>
        /// Check that absolute link contains domain name
        /// </summary>
        private Uri _domain;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        public SelfLinkRule(Uri domain)
        {
            _domain = domain;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool allow(Uri url)
        {
            return ( string.Compare(url.Host,_domain.Host, true) == 0 );
        }

    }
}
