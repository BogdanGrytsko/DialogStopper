using System.Threading.Tasks;
using Xunit;

namespace PlayerMap.BasketballReference
{
    public class Runner
    {
        [Fact]
        public async Task Scrape()
        {
            await new Scraper().DoWork();
        }
    }
}