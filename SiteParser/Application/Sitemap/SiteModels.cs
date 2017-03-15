using System;   

namespace SiteParser.Application.Sitemap
{
    class SiteMapRow
    {
        private string _location = null;
        private string _lastmod = null;
        private string _changefreq = null;
        private string _priority = null;

        public string location
        {
            get
            {
                return _location
                    .Replace("&", "&amp;")
                    .Replace("'", "&apos;")
                    .Replace("\"","&quot;")
                    .Replace(">","&gt;")
                    .Replace("<","&lt");
            }
            set
            {
                _location = value;
            }
        }
        public string lastmod {
            get
            {
                return _lastmod == null
                    ? DateTime.Now.ToString("yyyy-mm-dd")
                    : _lastmod;
            }
            set
            {
                _lastmod = (value != null && string.IsNullOrEmpty(value))
                    ? DateTime.Parse(value).ToString("yyyy-MM-dd")
                    : DateTime.Now.ToString("yyyy-MM-dd");  
            }
        }
        public string chanagefreq {
            get
            {
                return _changefreq == null
                    ? "daily"
                    : _changefreq;
            }
            set
            {
                _changefreq = value;
            }
        }
        public string priority {
            get
            {
                return _priority == null
                    ? "1"
                    : _priority;
            }
            set
            {
                _priority = value;
            }
        }
    }
}
