using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference.Model
{
    internal class TeamDtoSRefMap : ClassMap<TeamDto>
    {
        public TeamDtoSRefMap()
        {
            Map(x => x.BBRefTeamId).Name("BBRefTeamId");
            Map(x => x.TeamName).Name("TeamName");
            Map(x => x.MongoTeamId).Name("MongoTeamId");
        }
    }
}

