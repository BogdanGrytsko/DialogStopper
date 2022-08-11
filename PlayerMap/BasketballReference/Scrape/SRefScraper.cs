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
            var teamsData = DataExporter.Import(@"C:\temp\Sportradar\SRefTeams.csv");
            var teams = new DataImporter<TeamDto, TeamDtoSRefMap>().LoadData(teamsData, ";");
            var seasonsData = Helper.GetResource(
                Assembly.GetExecutingAssembly(), "PlayerMap.BasketballReference.NBA.BBRef to Mongo Season ID.csv");
            var seasons = new DataImporter<SeasonDto, SeasonDtoMap>().LoadData(seasonsData, ";");
            var batchSize = 100;
            foreach (var season in seasons)
            {
                for (int i = 0; i < teams.Count; i = i + batchSize)
                {
                    var players = new List<BBRefPlayer>();
                    var items = teams.Skip(i).Take(batchSize);
                    foreach (var team in items)
                    {
                        await Scrape(season, team, players);
                    }

                    DataExporter.Export(players,
                        $@"C:\temp\Sportradar\SRefPlayers_{i}-{i + batchSize}Teams-{season.BBRefSeasonName}.csv");
                }
            }
        }

        private static async Task Scrape(SeasonDto season, TeamDto team, List<BBRefPlayer> bbRefPlayers)
        {
            var url = $@"https://www.sports-reference.com{team.BBRefTeamId}{season.BBRefSeasonId}.html";
            var doc = await new HtmlWeb().LoadFromWebAsync(url);
            if (BBRefScraper.PageNotFound(doc))
                return;
            var table = doc.DocumentNode.SelectNodes(@"//table[@id='roster']//tr");
            if (table == null)
                return;
            //skip headers
            var rows = table.Skip(1);
            foreach (var row in rows)
            {
                var player = RoasterTableParser.GetSRefPlayer(row);
                player.TeamName = team.TeamName;
                player.MongoTeamId = team.MongoTeamId;
                player.SeasonName = season.BBRefSeasonName;
                player.MongoSeasonId = season.MongoSeasonId;
                bbRefPlayers.Add(player);
            }
        }
    }
}