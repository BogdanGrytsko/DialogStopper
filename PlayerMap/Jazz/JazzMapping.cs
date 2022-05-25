using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PlayerMap.Model.MasterPl;

namespace PlayerMap.Jazz
{
    public class JazzMapping
    {
        public static void Enhance(List<MasterPlayerResult> masterPlayers)
        {
            var path = @"C:\temp\mappings_players.json";
            var data = File.ReadAllText(path);
            var jazzMap = JsonConvert.DeserializeObject<List<JazzDto>>(data);

            var masterPlayerMap = new Dictionary<uint, MasterPlayerResult>();
            foreach (var masterPlayer in masterPlayers)
            {
                foreach (var playerInfo in masterPlayer.PlayerInfos)
                {
                    if (!masterPlayerMap.ContainsKey(playerInfo.Player.iid))
                        masterPlayerMap.Add(playerInfo.Player.iid, masterPlayer);
                }
            }

            var jazzGroups = jazzMap.GroupBy(x => x.SourcePlayerId);
            foreach (var jazzGroup in jazzGroups)
            {
                MasterPlayerResult mp = null;
                foreach (var jazzDto in jazzGroup)
                {
                    if (uint.TryParse(jazzDto.TargetPlayerId, out var targetPlayerId) && masterPlayerMap.TryGetValue(targetPlayerId, out var masterPlayer))
                    {
                        masterPlayer.JazzId = jazzGroup.Key;
                        if (mp == null)
                            mp = masterPlayer;
                        else
                        {
                            if (mp != masterPlayer)
                            {
                                var comment = "Jazz mapped to a different player also";
                                if (!string.IsNullOrEmpty(masterPlayer.Comment))
                                    masterPlayer.Comment += $". {comment}";
                                else
                                    masterPlayer.Comment = comment;
                                break;
                            }                                
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Unknown mapping from Jazz side {jazzDto.TargetPlayerId}");
                    }
                }
            }
        }
    }
}