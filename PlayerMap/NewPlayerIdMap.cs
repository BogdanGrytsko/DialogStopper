using System;
using System.Collections.Generic;
using System.Linq;
using DialogStopper.Storage;
using PlayerMap.Model;

namespace PlayerMap
{
    public class NewPlayerIdMap
    {
        public static List<MasterPlayer> Map()
        {
            var players = new DataImporter<Player, MySqlPlayerMap>().LoadData(@"Data\\tblplayers.csv").ToList();
            var playerMap = players.ToDictionary(x => x.PlayerIid);
            var result = new List<MasterPlayer>();
            foreach (var player in players)
            {
                if (!playerMap.TryGetValue(player.NewPlayerIid, out var newPlayer) || player.PlayerIid == newPlayer.PlayerIid) continue;
                player.NewPlayer = newPlayer;
                newPlayer.ParentPlayer = player;
            }

            //roots
            var roots = players.Where(x => x.ParentPlayer == null && x.NewPlayerIid != 0).ToList();
            foreach (var player in roots)
            {
                var masterPlayer = new MasterPlayer();
                var currPlayer = player;
                do
                {
                    masterPlayer.Players.Add(currPlayer);
                    masterPlayer.PlayerIds.Add(currPlayer.PlayerIid);
                    if (masterPlayer.Players.Count > 10000)
                        throw new Exception("loop occured");
                    currPlayer = currPlayer.NewPlayer;

                } while (currPlayer != null && !masterPlayer.PlayerIds.Contains(currPlayer.PlayerIid));

                result.Add(masterPlayer);
            }

            //bad data - id may not lead to anywhere
            result = result.Where(x => x.PlayerIds.Count > 1).ToList();
            return result;
        }
    }
}