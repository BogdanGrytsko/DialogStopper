using System.Linq;
using FluentAssertions;
using Xunit;

namespace PlayerMap
{
    public class MapTest
    {
        [Fact]
        public void MongoPlayerMap()
        {
            MongoPlayerMapping.Run();
        }

        [Fact]
        public void MySqlPlayerMap()
        {
            var mySqlMasterPlayers = MySqlPlayerIdMap.Map();
            mySqlMasterPlayers.Count.Should().Be(1784);
            foreach (var masterPlayer in mySqlMasterPlayers)
            {
                masterPlayer.PlayerIids.Count.Should().BeGreaterThan(1);
            }

            var mongoMasterPlayers = MongoPlayerMapping.GetMasterPlayers();
            var nbaPlayers = mongoMasterPlayers.Where(x => x.Players.Any(y => y.LeagueId == "54457dce300969b132fcfb34")).ToList();

            var strangePlayers15 = nbaPlayers.Where(x => x.Players.Count > 15).ToList();
        }
    }
}