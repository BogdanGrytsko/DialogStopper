using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference.Model
{
    public class BBRefPlayerMap: ClassMap<BBRefPlayer>
    {
        public BBRefPlayerMap()
        {
            Map(x => x.Number).Name("Number");
            Map(x => x.Name).Name("Name");
            Map(x => x.Url).Name("Url");
            Map(x => x.Pos).Name("Pos");
            Map(x => x.Height).Name("Height");
            Map(x => x.Weight).Name("Weight");
            Map(x => x.BirthDate).Name("BirthDate");
            Map(x => x.BirthCountry).Name("BirthCountry");
            Map(x => x.YearsOfExperience).Name("YearsOfExperience");
            Map(x => x.College).Name("College");
            Map(x => x.CollegeUrl).Name("CollegeUrl");
            Map(x => x.Class).Name("Class");
            Map(x => x.IsNBA).Name("IsNBA");
            Map(x => x.RSCITop100).Name("RSCITop100");
            Map(x => x.Summary).Name("Summary");

            Map(x => x.TeamName).Name("TeamName");
            Map(x => x.MongoTeamId).Name("MongoTeamId");
            Map(x => x.SeasonName).Name("SeasonName");
            Map(x => x.MongoSeasonId).Name("MongoSeasonId");

        }
    }
}

