using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DialogStopper.Storage;
using Newtonsoft.Json;
using PlayerMap.Model;
using PlayerMap.Model.MasterPl;

namespace PlayerMap
{
    public class MongoPlayerMapping
    {
        public static void Run()
        {
            var zip = ZipFile.Open(@"C:\temp\temp.zip", ZipArchiveMode.Read);
            var players = GetPlayers(zip.Entries.Single(x => x.Name == "sport.players.csv"));
            var boxScores = GetBoxScoresMap(zip.Entries.Single(x => x.Name == "allBoxScores.csv"));

            var groups = players.GroupBy(x => x.GetKey()).OrderBy(x => x.Key).ToList();
            var matchedGroups = groups.Where(x => x.Count() > 1).OrderBy(x => x.Key).ToList();
            var singleGroupPlayers = groups.Count - matchedGroups.Count;
            var sb = new StringBuilder();
            sb.AppendLine($"Key;Id;Name;League;LeagueId;Season;SeasonId;BirthDate;BirthPlace;College;Debut;Height;Weight;Team;Iid");
            foreach (var group in groups)
            {
                foreach (var p in group)
                {
                    if (!boxScores.TryGetValue(p.Id, out var playerBs))
                        playerBs = new HashSet<BoxScore> { new() };
                    
                    foreach (var bs in playerBs)
                    {
                        sb.AppendLine($"{group.Key};{p.Id};{p.Name};{bs.LeagueName};{bs.LeagueId};{bs.SeasonName};{bs.SeasonId};{p.BirthDate};{p.BirthPlace};{p.College};{p.Debut};{p.Height};{p.Weight};{p.Team};{p.Iid}");    
                    }
                }
            }
            File.WriteAllText(@"C:\temp\master.players.csv", sb.ToString());
        }

        private static List<Player> GetPlayers(ZipArchiveEntry zipArchiveEntry)
        {
            using var reader = new StreamReader(zipArchiveEntry.Open());
            var players = new DataImporter<Player, MongoPlayerMap>().LoadData(reader, ";");
            foreach (var player in players)
            {
                player.Name = Clean(player.Name);
            }
            
            players = players.Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList();
            return players;
        }

        private static string Clean(string name)
        {
            var rgx = new Regex("[^a-zA-Z0-9 -]");
            name = rgx.Replace(name, "");
            return name.Trim();
        }

        private static Dictionary<string, HashSet<BoxScore>> GetBoxScoresMap(ZipArchiveEntry zipArchiveEntry)
        {
            using var reader = new StreamReader(zipArchiveEntry.Open());
            var boxScores = new DataImporter<BoxScore, BoxScoreMap>().LoadData(reader, ";");
            
            var dic = new Dictionary<string, HashSet<BoxScore>>();
            foreach (var boxScore in boxScores)
            {
                if (!dic.ContainsKey(boxScore.PlayerId))
                    dic.Add(boxScore.PlayerId, new HashSet<BoxScore>());
                dic[boxScore.PlayerId].Add(boxScore);
            }
            return dic;
        }

        public static List<MasterPlayer> GetMasterPlayers()
        {
            var mongoPlayers = new DataImporter<Player, MongoMasterPlayerMap>().LoadData(@"C:\temp\master.players.csv", ";").ToList();
            var mongoMasterPlayers = new List<MasterPlayer>();
            foreach (var group in mongoPlayers.GroupBy(x => x.GetKey()))
            {
                var mp = new MasterPlayer
                {
                    Key = group.Key
                };
                foreach (var player in group)
                {
                    if (!mp.PlayerIds.Contains(player.Id))
                    {
                        mp.Players.Add(player);
                        mp.PlayerIds.Add(player.Id);
                        mp.PlayerIids.Add(player.Iid);
                    }
                    if (!string.IsNullOrEmpty(player.Season))
                        mp.LeagueSeasons.Add(player.GetLeagueSeason());
                }
                mp.LeagueSeasons = mp.LeagueSeasons.OrderBy(x => x.Season.Name).ThenBy(x => Leagues.GetLeagueValue(x.League.id)).ToList();
                mongoMasterPlayers.Add(mp);
            }

            return mongoMasterPlayers;
        }

        public static void Save(List<MasterPlayer> players)
        {
            var json = JsonConvert.SerializeObject(players);
            File.WriteAllText(@"C:\temp\master.players.result.json", json);
        }
    }
}