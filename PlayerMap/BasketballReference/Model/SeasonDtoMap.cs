using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference.Model
{
    public class SeasonDtoMap : ClassMap<SeasonDto>
    {
        public SeasonDtoMap()
        {
            Map(x => x.BBRefSeasonId).Name("BBRef Season ID").Optional();
            Map(x => x.BBRefSeasonName).Name("Season Name").Optional();
            Map(x => x.MongoSeasonId).Name("Synergy Season ID").Optional();

            Map(x => x.MongoSeasonLeagueId).Name("league._id");
            Map(x => x.MongoSeasonId).Name("_id");
            Map(x => x.BBRefSeasonName).Name("iid");
        }
    }
}