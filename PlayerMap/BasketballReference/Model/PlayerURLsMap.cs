using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerMap.BasketballReference.Model
{
    public class PlayerURLsMap : ClassMap<UrlClass>
    {
        public PlayerURLsMap()
        {
            Map(x => x.Url).Name("url");
        }
    }

    public class UrlClass
    {
        public string Url { get; set; }
    }
}
