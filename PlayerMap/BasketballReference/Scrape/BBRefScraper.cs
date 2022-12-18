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
    public class BBRefScraper
    {

        private readonly Random rand = new Random();
        
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
                    var result = await Scrape(season, team);
                    players.AddRange(result);
                }
            }
            DataExporter.Export(players, @"C:\temp\Sportradar\BBRefPlayers.csv");
        }

        public async Task ScrapeAdditional()
        {
            var outputFile = @"C:\temp\Sportradar\BBRefPlayers Additional.csv";
            var teamsSeasonsData = Helper.GetResource(
                Assembly.GetExecutingAssembly(),
                "PlayerMap.BasketballReference.NBA.Additional BBREF NBA Season and Teams to Scrape.csv");
            var teamSeasons = new DataImporter<TeamSeasonDto, TeamSeasonDtoMap>().LoadData(teamsSeasonsData, ";");
            var scrapedPlayers = new DataImporter<BBRefPlayer, BBRefPlayerMap>().LoadData(outputFile, ";");
            var scrapedSeasons = scrapedPlayers.Select(x => x.MongoTeamId + x.MongoSeasonId).ToHashSet();
            var notScrapedSeasons = teamSeasons.Where(x => !scrapedSeasons.Contains(x.SynergyTeamId + x.SynergySeasonId))
                .ToList();
            
            var players = new List<BBRefPlayer>();
            players.AddRange(scrapedPlayers);
            foreach (var teamSeason in notScrapedSeasons)
            {
                var season = new SeasonDto
                {
                    MongoSeasonId = teamSeason.SynergySeasonId,
                    BBRefSeasonId = teamSeason.BBRefSeason,
                    BBRefSeasonName = teamSeason.BBRefSeason.ToString()
                };
                var team = new TeamDto
                {
                    MongoTeamId = teamSeason.SynergyTeamId,
                    TeamName = teamSeason.BBRefTeamId,
                    BBRefTeamId = teamSeason.BBRefTeamId
                };
                var result = await Scrape(season, team);
                if (!result.Any())
                {
                    Console.WriteLine($"No players for {teamSeason}");
                }
                players.AddRange(result);
                await Task.Delay(TimeSpan.FromSeconds(rand.Next(1, 10)));
            }

            
            DataExporter.Export(players, outputFile);
        }

        private static async Task<List<BBRefPlayer>> Scrape(SeasonDto season, TeamDto team)
        {
            var url = $@"https://www.basketball-reference.com/teams/{team.BBRefTeamId}/{season.BBRefSeasonId}.html";
            var doc = await new HtmlWeb().LoadFromWebAsync(url);
            if (PageNotFound(doc))
                return new List<BBRefPlayer>();
            if (doc.Text == "error code: 1015")
            {
                return new List<BBRefPlayer>();
                throw new Exception("We have been blocked :(");
            }

            var result = new List<BBRefPlayer>();
            //skip headers
            var rows = doc.DocumentNode.SelectNodes(@"//table[@id='roster']//tr").Skip(1);
            foreach (var row in rows)
            {
                var player = RoasterTableParser.GetPlayer(row);
                player.TeamName = team.TeamName;
                player.MongoTeamId = team.MongoTeamId;
                player.SeasonName = season.BBRefSeasonName;
                player.MongoSeasonId = season.MongoSeasonId;
                result.Add(player);
            }

            return result;
        }

        public static bool PageNotFound(HtmlDocument doc)
        {
            var h1 = doc.DocumentNode.SelectNodes(@"//h1");
            return h1 != null && h1.First().InnerText
                .Equals("Page Not Found (404 error)", StringComparison.OrdinalIgnoreCase);
        }
    }
}