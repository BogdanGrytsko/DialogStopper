using DialogStopper.Storage;
using DialogStopper;
using PlayerMap.BasketballReference.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace PlayerMap.BasketballReference.Scrape
{
    public class GLeagueScraper
    {
        private readonly Random rand = new Random();

        private static readonly List<string> tableIds = new List<string>()
        {
            "nbdl_per_minute-sc",
            "nbdl_per_minute-pst",
            "nbdl_per_minute-reg"
        };

        public async Task Scrape()
        {
            var teamsData = Helper.GetResource(
                Assembly.GetExecutingAssembly(),
                "PlayerMap.BasketballReference.GLeague.G League - BBRef to Mongo Team ID Mapping.csv");
            var teams = new DataImporter<TeamDto, TeamDtoMap>().LoadData(teamsData, ";");
            var seasonsData = Helper.GetResource(
                Assembly.GetExecutingAssembly(),
                "PlayerMap.BasketballReference.GLeague.G League - BBRef to Mongo Season ID.csv");
            var seasons = new DataImporter<SeasonDto, SeasonDtoMap>().LoadData(seasonsData, ";");

            var result = new List<GLeaguePlayer>();
            foreach (var team in teams)
            {
                foreach (var season in seasons)
                {
                    var players = await Scrape(team, season);
                    if (players.Any())
                    {
                        result.AddRange(players);
                    }
                    else
                    {
                        Console.WriteLine(
                            $"No players found for season {season.BBRefSeasonId} and team {team.BBRefTeamId}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(rand.Next(1, 10)));
                }
            }

            DataExporter.Export(result, @"C:\Users\y.bartish.ext\Desktop\GLeaguePlayers.csv");
        }

        public static async Task<List<GLeaguePlayer>> Scrape(TeamDto team, SeasonDto season)
        {
            var url =
                $@"https://www.basketball-reference.com/gleague/teams/{team.BBRefTeamId}/{season.BBRefSeasonId}.html";
            var doc = await new HtmlWeb().LoadFromWebAsync(url);

            if (doc.Text == "error code: 1015")
            {
                throw new Exception("We have been blocked :(");
            }

            var result = new List<GLeaguePlayer>();

            var newHtml = doc.DocumentNode.OuterHtml.Replace("<!-", "").Replace("-->", "");
            if (string.IsNullOrEmpty(newHtml))
            {
                return result;
            }

            var newNode = HtmlNode.CreateNode(newHtml);
            var rows = GetRows(newNode);

            if (rows is null)
            {
                return result;
            }

            foreach (var row in rows)
            {
                var player = GLeagueTableParser.GetPlayer(row);
                player.MongoTeamId = team.MongoTeamId;
                player.BBRefTeamId = team.BBRefTeamId;
                player.BBRedSeasonId = season.BBRefSeasonId;
                player.MongoSeasonId = season.MongoSeasonId;
                result.Add(player);
            }

            return result;
        }

        private static IEnumerable<HtmlNode> GetRows(HtmlNode node)
        {
            foreach (var tableId in tableIds)
            {
                var result = node.SelectNodes($@"//table[@id='{tableId}']//tr")?.Skip(1);
                if (result is not null)
                {
                    return result;
                }
            }

            return new List<HtmlNode>();
        }
    }
}