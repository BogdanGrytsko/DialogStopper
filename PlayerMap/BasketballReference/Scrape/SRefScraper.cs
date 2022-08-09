using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DialogStopper;
using DialogStopper.Storage;
using HtmlAgilityPack;
using PlayerMap.BasketballReference.Model;

namespace PlayerMap.BasketballReference.Scrape
{
    public class SRefScraper
    {
        public async Task GetTeams()
        {
            var url = @"https://www.sports-reference.com/cbb/schools/";
            var doc = await new HtmlWeb().LoadFromWebAsync(url);
            var rows = doc.DocumentNode.SelectNodes(@"//table[@id='schools']//tr");
            var teams = new List<TeamDto>();
            foreach (var row in rows)
            {
                var team = SchoolsTableParser.GetTeam(row);
                if (team != null)
                    teams.Add(team);
            }
            DataExporter.Export(teams, @"C:\temp\Sportradar\SRefTeams.csv", ";");
        }
        
        public async Task Scrape()
        {
            var teamsData = Helper.GetResource(
                Assembly.GetExecutingAssembly(), "PlayerMap.BasketballReference.NBA.BBRef to Mongo Team ID Mapping.csv");
            var teams = new DataImporter<TeamDto, TeamDtoMap>().LoadData(teamsData, ";");
            var seasonsData = Helper.GetResource(
                Assembly.GetExecutingAssembly(), "PlayerMap.BasketballReference.NBA.BBRef to Mongo Season ID.csv");
            var seasons = new DataImporter<SeasonDto, SeasonDtoMap>().LoadData(seasonsData, ";");
            
            var players = new List<BBRefPlayer>();
            foreach (var season in seasons)
            {
                foreach (var team in teams)
                {
                    await Scrape(season, team, players);
                }
            }
            DataExporter.Export(players, @"C:\temp\Sportradar\BBRefPlayers.csv");
        }

        private static async Task Scrape(SeasonDto season, TeamDto team, List<BBRefPlayer> bbRefPlayers)
        {
            var url = $@"https://www.basketball-reference.com/teams/{team.BBRefTeamId}/{season.BBRefSeasonId}.html";
            var doc = await new HtmlWeb().LoadFromWebAsync(url);
            if (PageNotFound(doc))
                return;
            //skip headers
            var rows = doc.DocumentNode.SelectNodes(@"//table[@id='roster']//tr").Skip(1);
            foreach (var row in rows)
            {
                var player = RoasterTableParser.GetPlayer(row);
                player.TeamName = team.TeamName;
                player.MongoTeamId = team.MongoTeamId;
                player.SeasonName = season.BBRefSeasonName;
                player.MongoSeasonId = season.MongoSeasonId;
                bbRefPlayers.Add(player);
            }
        }

        private static bool PageNotFound(HtmlDocument doc)
        {
            var h1 = doc.DocumentNode.SelectNodes(@"//h1");
            return h1 != null && h1.First().InnerText
                .Equals("Page Not Found (404 error)", StringComparison.OrdinalIgnoreCase);
        }
    }
}