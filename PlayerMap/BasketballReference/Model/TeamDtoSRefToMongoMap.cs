using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference.Model
{
    internal class TeamDtoSRefToMongoMap : ClassMap<TeamDto>
    {
        public TeamDtoSRefToMongoMap()
        {
            Map(x => x.BBRefTeamId).Name("Sref TeamID From URL");
            Map(x => x.TeamName).Name("Team Name");
            Map(x => x.MongoTeamId).Name("Mongo TeamID");
        }
    }
}

