using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference.Model
{
    public class SeasonDtoMap : ClassMap<SeasonDto>
    {
        public SeasonDtoMap()
        {
            Map(x => x.BBRefSeasonId).Name("BBRef Season ID");
            Map(x => x.BBRefSeasonName).Name("Season Name");
            Map(x => x.MongoSeasonId).Name("Synergy Season ID");
        }
    }
}