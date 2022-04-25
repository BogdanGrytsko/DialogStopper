using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var mySqlMasterPlayers = MySqlPlayerIdMap.Map();
            // mySqlMasterPlayers.Count.Should().Be(1784);
            foreach (var masterPlayer in mySqlMasterPlayers)
            {
                masterPlayer.PlayerIids.Count.Should().BeGreaterThan(1);
            }

            var mongoMasterPlayers = MongoPlayerMapping.GetMasterPlayers();
            var nbaPlayers = mongoMasterPlayers
                .Where(x => x.Players.Any(y => y.LeagueId == "54457dce300969b132fcfb34"))
                .ToList();

            var mySqlDic = GetDictionary(mySqlMasterPlayers);
            mongoMasterPlayers.ForEach(x => x.AnalyzeCorrectness());
            foreach (var masterPlayer in mongoMasterPlayers)
            {
                if (!mySqlDic.TryGetValue(masterPlayer.Name, out var mySqlPlayers)) continue;
                foreach (var mySqlPlayer in mySqlPlayers.ToList())
                {
                    if (mySqlPlayer.Count != masterPlayer.Count) continue;
                    masterPlayer.IDSPlayerId = mySqlPlayer.GetDSPlayerId();
                    foreach (var leagueSeason in masterPlayer.LeagueSeasons)
                    {
                        leagueSeason.IDSPlayerId = mySqlPlayer.GetDSPlayerId();
                    }
                    mySqlPlayers.Remove(mySqlPlayer);
                }
            }

            MongoPlayerMapping.Save(mongoMasterPlayers);
        }

        private Dictionary<string, List<MasterPlayer>> GetDictionary(List<MasterPlayer> players)
        {
            var dic = new Dictionary<string, List<MasterPlayer>>();
            foreach (var player in players)
            {
                if (!dic.ContainsKey(player.Name))
                    dic.Add(player.Name, new List<MasterPlayer>());
                dic[player.Name].Add(player);
            }
            return dic;
        }
    }
}