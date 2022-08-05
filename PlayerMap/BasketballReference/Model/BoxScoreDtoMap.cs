using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference.Model
{
    public class BoxScoreDtoMap : ClassMap<BoxScoreDto>
    {
        public BoxScoreDtoMap()
        {
            Map(x => x.MongoSeasonId).Name("game.season._id");
            Map(x => x.MongoTeamId).Name("team._id");
            Map(x => x.PlayerName).Name("player.name");
            Map(x => x.MongoPlayerId).Name("player._id");
        }
    }
}