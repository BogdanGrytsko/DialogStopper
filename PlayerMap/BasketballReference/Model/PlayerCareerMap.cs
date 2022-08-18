using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference.Model
{
    public class PlayerCareerMap : ClassMap<PlayerCareer>
    {
        public PlayerCareerMap()
        {
            Map(x => x.MongoPlayer.MongoPlayerId).Name("MongoPlayerId");
            Map(x => x.MongoPlayer.Comment).Name("Comment");
            Map(x => x.MongoPlayer.Rating).Name("Rating");
            Map(x => x.MongoPlayer.SynergyPlayerName).Name("SynergyPlayerName").Optional();
            Map(x => x.BBRefPlayer.Number).Name("Number");
            Map(x => x.BBRefPlayer.Name).Name("Name");
            Map(x => x.BBRefPlayer.Url).Name("Url");
            Map(x => x.BBRefPlayer.Pos).Name("Pos");
            Map(x => x.BBRefPlayer.Height).Name("Height");
            Map(x => x.BBRefPlayer.Weight).Name("Weight");
            Map(x => x.BBRefPlayer.BirthDate).Name("BirthDate");
            Map(x => x.BBRefPlayer.BirthCountry).Name("BirthCountry");
            Map(x => x.BBRefPlayer.YearsOfExperience).Name("YearsOfExperience");
            Map(x => x.BBRefPlayer.College).Name("College");
            Map(x => x.BBRefPlayer.CollegeUrl).Name("CollegeUrl");
            Map(x => x.BBRefPlayer.Class).Name("Class").Optional();
            Map(x => x.BBRefPlayer.IsNBA).Name("IsNBA").Optional();
            Map(x => x.BBRefPlayer.RSCITop100).Name("RSCITop100").Optional();
            Map(x => x.BBRefPlayer.Summary).Name("Summary").Optional();


            Map(x => x.BBRefPlayer.TeamName).Name("TeamName");
            Map(x => x.BBRefPlayer.MongoTeamId).Name("MongoTeamId");
            Map(x => x.BBRefPlayer.SeasonName).Name("SeasonName");
            Map(x => x.BBRefPlayer.MongoSeasonId).Name("MongoSeasonId");

        }
    }
}