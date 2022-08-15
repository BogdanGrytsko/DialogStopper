using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference.Model
{
    public class SeasonDtoSRefToMongoMap : ClassMap<SeasonDto>
    {
        public SeasonDtoSRefToMongoMap()
        {
            Map(x => x.BBRefSeasonId).Name("Sref Season");
            Map(x => x.BBRefSeasonName).Name("Season Name");
            Map(x => x.MongoSeasonId).Name("Mongo Season");
        }
    }
}