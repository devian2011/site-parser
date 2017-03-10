using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteParser.Services.Parser.SiteMap
{
    class SiteMapRowModel
    {
        public string location { get; set; }
        public string lastModified { get; set; }
        public string changeFrequency { get; set; }
        public string priority { get; set; }
    }
}
