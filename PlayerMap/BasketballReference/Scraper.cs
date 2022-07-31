using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DialogStopper;
using DialogStopper.Storage;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PlayerMap.BasketballReference.Model;
using Xunit;

namespace PlayerMap.BasketballReference
{
    public class Scraper
    {
        [Fact]
        public async Task DoWork()
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
            var json = JsonConvert.SerializeObject(players);
            await File.WriteAllTextAsync(@"C:\temp\Sportradar\BBRefPlayers.json", json);
        }

        private async Task Scrape(SeasonDto season, TeamDto team, List<BBRefPlayer> bbRefPlayers)
        {
            var url = $@"https://www.basketball-reference.com/teams/{team.BBRefTeamId}/{season.BBRefSeasonId}.html";
            var doc = await new HtmlWeb().LoadFromWebAsync(url);
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
    }
}