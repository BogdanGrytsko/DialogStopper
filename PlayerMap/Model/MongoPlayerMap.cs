using CsvHelper.Configuration;

namespace PlayerMap.Model
{
    public class MongoPlayerMap : ClassMap<Player>
    {
        public MongoPlayerMap()
        {
            Map(m => m.Id).Name("_id");
            Map(m => m.Iid).Name("iid");
            Map(m => m.Name).Name("name");
            Map(m => m.BirthDate).Name("birthDate");
            Map(m => m.BirthPlace).Name("birthPlace");
            Map(m => m.College).Name("college");
            Map(m => m.Debut).Name("debut");
            Map(m => m.Height).Name("height");
            Map(m => m.Weight).Name("weight");
            Map(m => m.Team).Name("team");
            Map(m => m.Number).Name("number");
        }
    }
}