using CsvHelper;
using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference.Model
{
    public class MongoPlayerDtoMap : ClassMap<MongoPlayerDto>
    {
        public MongoPlayerDtoMap()
        {
            Map(x => x.MongoTeamName).Name("team.name");
            Map(x => x.MongoTeamAbbr).Name("team.abbr");
            Map(x => x.MongoTeamId).Name("team._id");
            Map(x => x.Name).Name("name");
            Map(x => x.Number).Name("number").Optional();
            Map(x => x.Id).Name("_id");
        }
    }
}