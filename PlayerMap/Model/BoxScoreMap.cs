using CsvHelper.Configuration;

namespace PlayerMap.Model
{
    public class BoxScoreMap : ClassMap<LeagueSeason>
    {
        public BoxScoreMap()
        {
            Map(m => m.LeagueId).Name("game.league._id");
            Map(m => m.LeagueName).Name("game.league.name");
            Map(m => m.SeasonId).Name("game.season._id");
            Map(m => m.SeasonName).Name("game.season.name");
            Map(m => m.PlayerId).Name("player._id");
        }
    }
}