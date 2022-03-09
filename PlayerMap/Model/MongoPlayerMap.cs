using CsvHelper.Configuration;

namespace PlayerMap.Model
{
    public class MongoPlayerMap : ClassMap<Player>
    {
        public MongoPlayerMap()
        {
            Map(m => m.Key).Name("Key");
            
            Map(m => m.Id).Name("Id");
            Map(m => m.Name).Name("Name");
            Map(m => m.League).Name("League");
            Map(m => m.LeagueId).Name("LeagueId");
            Map(m => m.Season).Name("Season");
            Map(m => m.SeasonId).Name("SeasonId");
            
            Map(m => m.BirthDate).Name("BirthDate");
            Map(m => m.BirthPlace).Name("BirthPlace");
            Map(m => m.College).Name("College");
            Map(m => m.Debut).Name("Debut");
            Map(m => m.Height).Name("Height");
            Map(m => m.Weight).Name("Weight");
            Map(m => m.Team).Name("Team");
        }
    }
}