using CsvHelper.Configuration;

namespace PlayerMap.Model.Scrape
{
    public class NBAPlayerMap : ClassMap<NbaPlayer>
    {
        public NBAPlayerMap()
        {
            Map(m => m.TeamName).Name("TeamName");
            Map(m => m.Username).Name("Username");
            Map(m => m.PlayerUrl).Name("PlayerUrl");
            Map(m => m.Weight).Name("Weight");
            Map(m => m.Height).Name("Height");
            Map(m => m.Born).Name("Born");
            Map(m => m.College).Name("College");
            Map(m => m.HighSchool).Name("HighSchool");
            Map(m => m.NBADebut).Name("NBADebut");
            Map(m => m.Experience).Name("Expirience");
        }
    }
}