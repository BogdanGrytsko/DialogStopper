using System.Linq;
using HtmlAgilityPack;
using PlayerMap.BasketballReference.Model;

namespace PlayerMap.BasketballReference.Scrape
{
    public class SchoolsTableParser
    {
        public static TeamDto GetTeam(HtmlNode row)
        {
            var teamNode = row.SelectNodes(RoasterTableParser.ColumnA("school_name"));
            if (teamNode == null)
                return null;
            var team = teamNode.First();
            return new TeamDto
            {
                BBRefTeamId = team.Attributes["href"].Value,
                TeamName = team.InnerText
            };
        }
    }
}