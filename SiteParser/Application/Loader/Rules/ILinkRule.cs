using System;

namespace SiteParser.Application.Loader.Rules
{
    interface ILinkRule
    {
        bool allow(Uri url);
    }
}
