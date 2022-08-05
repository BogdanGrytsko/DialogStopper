using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference.Model
{
    public class MongoPlayerDtoMap : ClassMap<MongoPlayerDto>
    {
        public MongoPlayerDtoMap()
        {
            Map(x => x.MongoTeamId).Name("team._id");
            Map(x => x.Name).Name("name");
            Map(x => x.Number).Name("number");
            Map(x => x.Id).Name("_id");
        }
    }
}