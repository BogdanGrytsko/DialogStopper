using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PlayerMap.BasketballReference.Model;
using PlayerMap.BasketballReference.Scrape;
using Xunit;
using System.Linq;
using DialogStopper.Storage;
using FuzzySharp;
using Google.Apis.Sheets.v4.Data;

namespace PlayerMap.BasketballReference
{
    public class Runner
    {
        [Fact]
        public async Task ScrapeBBRef()
        {
            await new BBRefScraper().Scrape();
        }

        [Fact]
        public async Task ScrapeBBRefAdditional()
        {
            await new BBRefScraper().ScrapeAdditional();
        }

        [Fact]
        public async Task Map()
        {
            var scrapedPlayersPath = @"C:\temp\Sportradar\BBRefPlayers.csv";
            var mongoPlayersPath = @"C:\temp\Sportradar\AllPlayers.csv";
            var boxScoresPath = @"C:\temp\Sportradar\BoxScoresNBA.csv";
            var fileName = "PlayerCareersNBA";
            await new BBRefMapper().Map(scrapedPlayersPath, mongoPlayersPath, boxScoresPath, fileName);
        }

        [Fact]
        public async Task MapNbaAdditional()
        {
            var scrapedPlayersPath = @"C:\temp\Sportradar\BBRefPlayers Additional.csv";
            var mongoPlayersPath = @"C:\temp\Sportradar\AllPlayers.csv";
            var boxScoresPath = @"C:\temp\Sportradar\BoxScoresNBA.csv";
            var fileName = "PlayerCareersNBA Additional";
            await new BBRefMapper().Map(scrapedPlayersPath, mongoPlayersPath, boxScoresPath, fileName);
        }

        [Fact]
        public async Task MapCMRef()
        {
            var scrapedPlayersPath = @"C:\Temp\SportRadar\SRefPlayers.csv";
            var mongoPlayersPath = @"C:\temp\Sportradar\AllPlayers.csv";
            var boxScoresPath = @"C:\temp\Sportradar\BoxScoresCollegeMan.csv";
            var fileName = "PlayerCareersCMRef";
            await new BBRefMapper().Map(scrapedPlayersPath, mongoPlayersPath, boxScoresPath, fileName);
        }

        [Fact]
        public async Task ScrapeSRefTeams()
        {
            await new SRefScraper().GetTeams();
        }

        [Fact]
        public async Task ScrapeSRefTeamBBRefSeasons()
        {
            await new SRefScraper().ScrapeSRefTeamBBRefSeasonsBatchSize();
        }

        [Fact]
        public async Task ScrapeSRef()
        {
            await new SRefScraper().ScrapeSRefToMongoTeamSRefToMongoSeasons();
        }

        [Fact]
        public async Task MapNBAToCollege()
        {
            await new MapNBAToCollege().Map();
        }

        private static Random rand = new Random();


        private const string NBAGLeague = "54457dce300969b132fcfb36";
        private const string CollegeMan = "54457dce300969b132fcfb37";
        private List<MongoPlayerDto> playersWithTeams;
        private List<SeasonDto> seasons;
        [Fact]
        public async Task NBAAllData()
        {
            var playerUrlsPath = @"C:\Users\y.bartish.ext\Desktop\SportRadar\InputFiles\PlayerURLs.csv";
            var playersWithTeamPath = @"C:\Users\y.bartish.ext\Desktop\SportRadar\InputFiles\AllPlayersWithTeams.csv";
            var seasonsPath = @"C:\Users\y.bartish.ext\Desktop\SportRadar\InputFiles\NBA G-League seasons.csv";

            var playerUrls = new DataImporter<UrlClass, PlayerURLsMap>().LoadData(playerUrlsPath).ToList();
            playersWithTeams = new DataImporter<MongoPlayerDto, MongoPlayerDtoMap>().LoadData(playersWithTeamPath, delimiter: ";")
                .ToList();
            seasons = new DataImporter<SeasonDto, SeasonDtoMap>().LoadData(seasonsPath, delimiter:";").ToList();


            var playerCareers = new List<NBAUpdatePlayerCareer>();

            foreach (var playerUrl in playerUrls)
            {
                var playerCareer = new NBAUpdatePlayerCareer
                {
                    Url = playerUrl.Url,
                    Items = new List<CareerItem>()
                };

                var url = $@"https://www.basketball-reference.com{playerUrl.Url}";
                var doc = await new HtmlWeb().LoadFromWebAsync(url);

                if (doc.Text == "error code: 1015")
                {
                    continue;
                }

                await Task.Delay(TimeSpan.FromSeconds(rand.Next(10)));

                var urls = doc.DocumentNode.SelectNodes(@"//ul[@class='']//li")
                    .TakeLast(3).Select(x => x.SelectSingleNode(@"a").Attributes["href"].Value).ToList();

                var playerName = doc.DocumentNode.SelectNodes(@"//div[@id='info']//span").FirstOrDefault();

                playerCareer.PlayerName = playerName?.InnerText;

                //await GetCareerItemsCollege(playerCareer, urls.FirstOrDefault(x => x.Contains("cbb")));
                await GetCareerItemsGleague(playerCareer, urls.FirstOrDefault(x => x.Contains("gleague")));
                //await GetCareerItemsInternational(playerCareer, urls.FirstOrDefault(x => x.Contains("international")));

                if (playerCareer.Items.Any())
                {
                    playerCareers.Add(playerCareer);
                }

            }

            var dataToExport = playerCareers.SelectMany(x => x.Items.Select(y => new
            {
                y.LeagueName,
                y.MongoLeagueId,
                y.SeasonName,
                y.MongoSeasonId,
                y.TeamName,
                y.MongoTeamId,
                x.PlayerName,
                x.Url,
            })).ToList();


            DataExporter.Export(dataToExport, @"C:\Users\y.bartish.ext\Desktop\NBAPlayerCareersUpdate.csv");
        }

        private async Task GetCareerItemsCollege(NBAUpdatePlayerCareer career, string pageUrl)
        {
            if (string.IsNullOrEmpty(pageUrl))
            {
                return;
            }
            var doc = await new HtmlWeb().LoadFromWebAsync(pageUrl);
            if (doc.Text == "error code: 1015")
            {
                return;
            }
            var rows = doc.DocumentNode.SelectNodes(@"//table[@id='players_totals']//tr").Skip(1).SkipLast(1).ToList();

            if (rows is null)
            {

            }

            var careerItems = rows.Select(x => new CareerItem()
            {
                MongoLeagueId = CollegeMan,
                LeagueName = "College Men",
                SeasonName = x.SelectSingleNode(@"th[@data-stat='season']//a")?.InnerHtml,
                TeamName = x.SelectSingleNode(@"td[@data-stat='school_name']//a")?.InnerText
            });
            career.Items.AddRange(careerItems);
            await Task.Delay(TimeSpan.FromSeconds(rand.Next(10)));
        }

        private async Task GetCareerItemsGleague(NBAUpdatePlayerCareer career, string pageUrl)
        {
            if (string.IsNullOrEmpty(pageUrl))
            {
                return;
            }
            var doc = await new HtmlWeb().LoadFromWebAsync(pageUrl);
            if (doc.Text == "error code: 1015")
            {
              return;
            }
            var newHtml = doc.DocumentNode.OuterHtml.Replace("<!-", "").Replace("-->", "");
            if (string.IsNullOrEmpty(newHtml))
            {
                return;
            }

            var newNode = HtmlNode.CreateNode(newHtml);
            //nbdl_totals-pst
            var rows = newNode.SelectNodes(@"//table[@id='nbdl_totals-reg']/tbody//tr");
            if (rows is null)
            {
                newNode.SelectNodes(@"//table[@id='nbdl_totals-pst']/tbody//tr");
            }

            if (rows is null)
            {
                return;
            }

            var careerItems = new List<CareerItem>();
            foreach (var row in rows)
            {
                var seasonName = row.SelectSingleNode(@"th[@data-stat='season']//a").InnerHtml;
                var teamName = row.SelectSingleNode(@"td[@data-stat='team_id']//a").InnerText;

                var careerItem = new CareerItem()
                {
                    MongoLeagueId = NBAGLeague,
                    LeagueName = "NBA G-League",
                    SeasonName = seasonName,
                    TeamName = teamName
                };

                careerItem.MongoSeasonId =
                    seasons.FirstOrDefault(x => seasonName.Contains(x.BBRefSeasonName))?.MongoSeasonId;
                careerItem.MongoTeamId = playersWithTeams
                    .FirstOrDefault(x => x.MongoTeamAbbr == teamName || x.MongoTeamName == teamName)?.MongoTeamId;

                careerItem.MongoPlayerId = playersWithTeams.FirstOrDefault(x =>
                    x.MongoTeamId == careerItem.MongoTeamId)?.Id;

                careerItems.Add(careerItem);
            }

            career.Items.AddRange(careerItems);
            await Task.Delay(TimeSpan.FromSeconds(rand.Next(10)));

        }

        private List<string> InternationalTableIds = new List<string>()
        {
            "player-stats-totals-all-",
            "player-stats-totals-tournament-",
            "player-stats-totals-league-"
        };

        private async Task GetCareerItemsInternational(NBAUpdatePlayerCareer career, string pageUrl)
        {
            if (string.IsNullOrEmpty(pageUrl))
            {
                return;
            }
            var doc = await new HtmlWeb().LoadFromWebAsync(pageUrl);
            if (doc.Text == "error code: 1015")
            {
                return;
            }


            foreach (var tableId in InternationalTableIds)
            {
                var rows = doc.DocumentNode.SelectNodes(@$"//table[@id='{tableId}']/tbody//tr");

                if (rows is null && tableId == InternationalTableIds.Last())
                {

                }
                if (rows is not null)
                {
                    var careerItems = rows?.Select(x => new CareerItem
                    {
                        SeasonName = x.SelectSingleNode(@"th[@data-stat='season']/a").InnerHtml,
                        TeamName = x.SelectSingleNode(@"td[@data-stat='team']/a").InnerText,
                        LeagueName = x.SelectSingleNode(@"td[@data-stat='league']/a").InnerText,
                    });
                    career.Items.AddRange(careerItems);
                    await Task.Delay(TimeSpan.FromSeconds(rand.Next(10)));
                    break;
                }
            }


        }
    }
}