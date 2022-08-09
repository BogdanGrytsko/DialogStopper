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
            await new BBRefMapper().Map();
        }

        [Fact]
        public async Task ScrapeSRefTeams()
        {
            await new SRefScraper().GetTeams();
        }
        
        [Fact]
        public async Task ScrapeSRef()
        {
            await new SRefScraper().Scrape();
        }
    }
}