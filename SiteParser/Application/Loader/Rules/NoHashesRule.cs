using System;

namespace SiteParser.Application.Loader.Rules
{
    class NoHashesRule : ILinkRule
    {

        public bool allow(Uri url)
        {
            return !url.OriginalString.Contains("#");
        }

    }
}
