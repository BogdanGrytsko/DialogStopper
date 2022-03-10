using System;
using System.Collections.Generic;
using System.Linq;
using DialogStopper.Storage;
using PlayerMap.Model;

namespace PlayerMap
{
    public class MySqlPlayerIdMap
    {
        public static List<MasterPlayer> Map()
        {
            var players = new DataImporter<Player, MySqlPlayerMap>().LoadData(@"Data\tblplayers.csv").ToList();
            var playerMap = players.ToDictionary(x => x.Iid);
            var result = new List<MasterPlayer>();
            foreach (var player in players)
            {
                if (!playerMap.TryGetValue(player.NewPlayerIid, out var newPlayer) || player.Iid == newPlayer.Iid) continue;
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
                    masterPlayer.PlayerIids.Add(currPlayer.Iid);
                    if (masterPlayer.Players.Count > 10000)
                        throw new Exception("loop occured");
                    currPlayer = currPlayer.NewPlayer;

                } while (currPlayer != null && !masterPlayer.PlayerIids.Contains(currPlayer.Iid));

                result.Add(masterPlayer);
            }

            //bad data - id may not lead to anywhere
            result = result.Where(x => x.PlayerIids.Count > 1).ToList();
            return result;
        }
    }
}