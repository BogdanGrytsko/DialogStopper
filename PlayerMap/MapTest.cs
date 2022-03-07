using FluentAssertions;
using PlayerMap.Model;
using Xunit;

namespace PlayerMap
{
    public class MapTest
    {
        [Fact(Skip = "Not now")]
        public void PlayerMap()
        {
            PlayerMapping.Run();
        }

        [Fact]
        public void NewPlayerMap()
        {
            var masterPlayers = NewPlayerIdMap.Map();
            masterPlayers.Count.Should().Be(1784);
            foreach (var masterPlayer in masterPlayers)
            {
                masterPlayer.PlayerIds.Count.Should().BeGreaterThan(1);
            }
        }
    }
}