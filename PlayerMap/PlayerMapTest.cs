using System.Collections.Generic;
using System.IO;
using System.Linq;
using DialogStopper.Storage;
using FluentAssertions;
using Newtonsoft.Json;
using PlayerMap.Jazz;
using PlayerMap.Model;
using PlayerMap.Model.MasterPl;
using PlayerMap.Model.Scrape;
using Xunit;

namespace PlayerMap
{
    public class PlayerMapTest
    {
        [Fact]
        public void MongoPlayerMap()
        {
            MongoPlayerMapping.Run();
        }

        [Fact]
        public void CombinedPlayerMap()
        {
            var mySqlMasterPlayers = MySqlPlayerIdMap.Map();
            mySqlMasterPlayers.ForEach(x => x.SetName());
            foreach (var masterPlayer in mySqlMasterPlayers)
            {
                masterPlayer.PlayerIids.Count.Should().BeGreaterThan(1);
            }

            var mongoMasterPlayers = MongoPlayerMapping.GetMasterPlayers();
            mongoMasterPlayers.ForEach(x => x.SetData());
            PlayerSplitter.Split(ref mongoMasterPlayers);
            
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

        [Fact]
        public void JazzNBAToCsv()
        {
            var path = @"C:\temp\master.players.result.json";
            var data = File.ReadAllText(path);
            var masterPlayers = JsonConvert.DeserializeObject<List<MasterPlayerResult>>(data);
            JazzMapping.Enhance(masterPlayers);
            NBAMapping.Enhance(masterPlayers);
            
            File.WriteAllText(path, JsonConvert.SerializeObject(masterPlayers));
            
            var flattened = new List<MasterPlayerFlat>();
            foreach (var m in masterPlayers)
            {
                foreach (var p in m.PlayerInfos)
                {
                    flattened.Add(new MasterPlayerFlat
                    {
                        Comment = m.Comment,
                        Correctness = m.Correctness,
                        Name = m.Name,
                        Key = m.Key,
                        IDSPlayerId = m.IDSPlayerId,
                        JazzId = m.JazzId,
                        NbaPlayer = m.NbaPlayer,
                        League = p.League,
                        Player = p.Player,
                        Season = p.Season,
                        Team = p.Team
                    });
                }
            }
            DataExporter.Export(flattened, path.Replace("json", "csv"));
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