using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference
{
    public class RatedMongoPlayerMap : ClassMap<RatedMongoPlayer>
    {
        public RatedMongoPlayerMap()
        {
            Map(x => x.MongoPlayerId).Name("MongoPlayerId");
            Map(x => x.Comment).Name("Comment");
            Map(x => x.Rating).Name("Rating");
        }
    }
}