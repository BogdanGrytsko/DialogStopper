using System.Collections.Generic;
using System.Linq;
using DialogStopper.Storage;
using PlayerMap.Model.MasterPl;

namespace PlayerMap.Model.Scrape
{
    public class NBAMapping
    {
        public static void Enhance(List<MasterPlayerResult> masterPlayers)
        {
            var path = @"C:\temp\NbaPlayersTeamsSanitized.csv";
            var players = new DataImporter<NbaPlayer, NBAPlayerMap>().LoadData(path, ",");
            HandleHeightWeight(players);
            var masterPlayerMap = masterPlayers
                .GroupBy(x => x.Name)
                .ToDictionary(x => x.Key, x => x.ToList());
            foreach (var player in players)
            {
                if (masterPlayerMap.TryGetValue(player.Username, out var mps))
                {
                    foreach (var mp in mps)
                    {
                        mp.NbaPlayer = player;
                    }
                }
            }
        }

        private static void HandleHeightWeight(List<NbaPlayer> players)
        {
            foreach (var player in players)
            {
                var sp = player.Height.Split(",");
                player.Height = sp[0];
                if (sp.Length > 1)
                    player.Weight = sp[1];
            }
        }
    }
}