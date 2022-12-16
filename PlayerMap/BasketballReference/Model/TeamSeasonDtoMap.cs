using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference.Model
{
    public class TeamSeasonDtoMap : ClassMap<TeamSeasonDto>
    {
        public TeamSeasonDtoMap()
        {
            Map(x => x.BBRefTeamId).Name("BB Ref Team ID");
            Map(x => x.BBRefSeason).Name("BBRef Season");
            Map(x => x.SynergySeasonId).Name("Synergy Season");
            Map(x => x.SynergyTeamId).Name("Synergy Team");
        }
    }
}