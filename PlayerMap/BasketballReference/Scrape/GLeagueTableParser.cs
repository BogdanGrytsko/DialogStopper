using HtmlAgilityPack;
using PlayerMap.BasketballReference.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PlayerMap.BasketballReference.Scrape
{
    public class GLeagueTableParser
    {
        public static GLeaguePlayer GetPlayer(HtmlNode row)
        {
            var name = row.SelectNodes(@"td[@data-stat='player']//a")?.FirstOrDefault()?.InnerText; 
            var playerUrl = row.SelectSingleNode(@"td[@data-stat='player']//a")?.Attributes["href"].Value;

            var player = new GLeaguePlayer()
            {
                Name = name,
                PlayerUrl = playerUrl
            };

            return player;
        }
    }
}
