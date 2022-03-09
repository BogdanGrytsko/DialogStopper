using System.Linq;
using DialogStopper.Storage;
using FluentAssertions;
using PlayerMap.Model;
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
            var masterPlayers = MySqlPlayerIdMap.Map();
            masterPlayers.Count.Should().Be(1784);
            foreach (var masterPlayer in masterPlayers)
            {
                masterPlayer.PlayerIds.Count.Should().BeGreaterThan(1);
            }
            var mongoPlayers = new DataImporter<Player, MongoPlayerMap>().LoadData(@"C:\temp\master.players.csv", ";").ToList();
            //so we would like to compare this results to default mapping, maybe do some improvements
            mongoPlayers.Count.Should().BeGreaterThan(500000);
            var nbaPlayers = mongoPlayers.Where(x => x.LeagueId == "54457dce300969b132fcfb34").ToList();
        }
    }
}