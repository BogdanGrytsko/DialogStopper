using PlayerMap.BasketballReference.Scrape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PlayerMap.BasketballReference
{
    public class GLeagueRunner
    {
        [Fact]
        public async Task ScrapeGLeague()
        {
            await new GLeagueScraper().Scrape();
        }

        [Fact]
        public async Task Map()
        {
            var scrapedPlayersPath = @"C:\temp\Sportradar\GLeaguePlayers.csv";
            var mongoPlayersPath = @"C:\temp\Sportradar\AllPlayers.csv";
            var boxScoresPath = @"C:\temp\Sportradar\BoxScoresGLeague.csv";
            var fileName = "PlayerCareersGLeague";
            await new GLeagueMapper().Map(scrapedPlayersPath, mongoPlayersPath, boxScoresPath, fileName);
        }
    }
}
