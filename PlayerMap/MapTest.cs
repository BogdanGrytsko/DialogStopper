using PlayerMap.Model;
using Xunit;

namespace PlayerMap
{
    public class MapTest
    {
        [Fact]
        public void PlayerMap()
        {
            PlayerMapping.Run();
        }

        [Fact]
        public void NewPlayerMap()
        {
            NewPlayerIdMap.Map();
        }
    }
}