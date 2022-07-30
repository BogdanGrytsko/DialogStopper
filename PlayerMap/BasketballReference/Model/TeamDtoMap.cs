using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference.Model
{
    public class TeamDtoMap : ClassMap<TeamDto>
    {
        public TeamDtoMap()
        {
            Map(x => x.BBRefTeamId).Name("Basketball Ref Team ID");
            Map(x => x.TeamName).Name("Team Name");
            Map(x => x.MongoTeamId).Name("Mongo Team ID");
        }
    }
}