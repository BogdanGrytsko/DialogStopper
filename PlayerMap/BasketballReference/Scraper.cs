using System.Reflection;
using System.Threading.Tasks;
using DialogStopper;
using DialogStopper.Storage;
using HtmlAgilityPack;
using PlayerMap.BasketballReference.Model;
using Xunit;

namespace PlayerMap.BasketballReference
{
    public class Scraper
    {
        [Fact]
        public async Task DoWork()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var teamsData = Helper.GetResource(assembly, "PlayerMap.BasketballReference.NBA.BBRef to Mongo Team ID Mapping.csv");
            var teams = new DataImporter<TeamDto, TeamDtoMap>().LoadData(teamsData, ";");
            var seasonsData = Helper.GetResource(assembly, "PlayerMap.BasketballReference.NBA.BBRef to Mongo Season ID.csv");
            var seasons = new DataImporter<SeasonDto, SeasonDtoMap>().LoadData(seasonsData, ";");
            foreach (var season in seasons)
            {
                foreach (var team in teams)
                {
                    await Scrape(season, team);
                }    
            }
        }

        private async Task Scrape(SeasonDto season, TeamDto team)
        {
            var url = $@"https://www.basketball-reference.com/teams/{team.BBRefTeamId}/{season.BBRefSeasonId}.html";
            var doc = await new HtmlWeb().LoadFromWebAsync(url);
            throw new System.NotImplementedException();
        }
    }
}