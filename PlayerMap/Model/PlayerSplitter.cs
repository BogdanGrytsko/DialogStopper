using System.Collections.Generic;
using System.Linq;
using PlayerMap.Model.MasterPl;

namespace PlayerMap.Model
{
    public class PlayerSplitter
    {
        public static void Split(ref List<MasterPlayer> mongoMasterPlayers)
        {
            var res = new List<MasterPlayer>();
            foreach (var player in mongoMasterPlayers)
            {
                var groups = player.LeagueSeasons.GroupBy(x => Leagues.IsWomanLeague(x.League.Name)).ToList();
                if (groups.Count == 1)
                {
                    res.Add(player);
                }
                else if (groups.Count == 2)
                {
                    foreach (var @group in groups)
                    {
                        var symbol = @group.Key ? "F" : "M";
                        var clone = new MasterPlayer { Name = player.Name, Key = player.Key + $"_{symbol}" };
                        clone.LeagueSeasons = @group.ToList();
                        res.Add(clone);
                    }
                }
            }

            mongoMasterPlayers = res;
        }
    }
}