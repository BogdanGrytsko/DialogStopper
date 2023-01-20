using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace PlayerMap.BasketballReference.Model
{
    public class GLeaguePlayerMap : ClassMap<GLeaguePlayer>
    {
        public GLeaguePlayerMap()
        {
            Map(x => x.Name).Name("Name");
            Map(x => x.PlayerUrl).Name("PlayerUrl");
            Map(x => x.BBRedSeasonId).Name("BBRedSeasonId");
            Map(x => x.BBRefTeamId).Name("BBRefTeamId");
            Map(x => x.MongoSeasonId).Name("MongoSeasonId");
            Map(x => x.MongoTeamId).Name("MongoTeamId");
        }
    }
}
