using DialogStopper;
using DialogStopper.Storage;
using PlayerMap.BasketballReference.Model;
using Xunit;

namespace PlayerMap.BasketballReference
{
    public class Scraper
    {
        [Fact]
        public void DoWork()
        {
            var teamsData = Helper.GetResource("PlayerMap.BasketballReference.NBA.BBRef to Mongo Team ID Mapping.csv");
            var teams = new DataImporter<TeamDto, TeamDtoMap>().LoadData(teamsData, ";");
            var seasonsData = Helper.GetResource("PlayerMap.BasketballReference.NBA.BBRef to Mongo Season ID.csv");
            var seasons = new DataImporter<SeasonDto, SeasonDtoMap>().LoadData(seasonsData, ";");
            
        }
    }
}