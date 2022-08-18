using System.Threading.Tasks;
using PlayerMap.BasketballReference.Scrape;
using Xunit;

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
        public async Task Map()
        {
            var scrapedPlayersPath = @"C:\temp\Sportradar\BBRefPlayers.csv";
            var mongoPlayersPath = @"C:\temp\Sportradar\AllPlayers.csv";
            var boxScoresPath = @"C:\temp\Sportradar\BoxScoresNBA.csv";
            var fileName = "PlayerCareersNBA";
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
    }
}