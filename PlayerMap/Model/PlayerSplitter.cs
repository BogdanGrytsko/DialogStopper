using System;
using System.Collections.Generic;
using System.Linq;
using PlayerMap.Model.MasterPl;

namespace PlayerMap.Model
{
    public class PlayerSplitter
    {
        public static void Split(ref List<MasterPlayer> mongoMasterPlayers)
        {
            DoSplit(SplitBySex, ref mongoMasterPlayers);
            DoSplit(SplitByYearGap, ref mongoMasterPlayers);
        }

        private static void DoSplit(Action<MasterPlayer, List<MasterPlayer>> splitBy, ref List<MasterPlayer> mongoMasterPlayers)
        {
            var res = new List<MasterPlayer>();
            foreach (var player in mongoMasterPlayers)
            {
                splitBy(player, res);
            }

            mongoMasterPlayers = res;
        }

        private static void SplitByYearGap(MasterPlayer player, List<MasterPlayer> res)
        {
            const int maxYearGap = 4;
            uint prevYear = 0;
            int prevIndex = 0;
            for (int i = 0; i < player.LeagueSeasons.Count; i++)
            {
                var currYear = player.LeagueSeasons[i].Season.iid;
                if (prevYear == 0)
                {
                    prevYear = currYear;
                    continue;
                }
                if (currYear - prevYear >= maxYearGap)
                {
                    AddPlayer(player, res, i, prevIndex, maxYearGap);
                    prevIndex = i;
                }

                prevYear = currYear;
            }
            
            if (prevIndex != 0)
                AddPlayer(player, res, player.LeagueSeasons.Count, prevIndex, maxYearGap);
            else
            //add what was there
                res.Add(player);
        }

        private static void AddPlayer(MasterPlayer player, List<MasterPlayer> res, int i, int prevIndex, int maxYearGap)
        {
            var clone = new MasterPlayer
            {
                Name = player.Name, Key = player.Key + $"_{i}",
                LeagueSeasons = player.LeagueSeasons.Skip(prevIndex).Take(i - prevIndex).ToList()
            };
            clone.Comments.Add($"Split by Year gap {maxYearGap - 1}");
            res.Add(clone);
        }

        private static void SplitBySex(MasterPlayer player, List<MasterPlayer> res)
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
                    var clone = new MasterPlayer
                    {
                        Name = player.Name, Key = player.Key + $"_{symbol}",
                        LeagueSeasons = @group.ToList()
                    };
                    clone.Comments.Add("Split by F/M");
                    res.Add(clone);
                }
            }
        }
    }
}