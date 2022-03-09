using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DialogStopper.Storage;
using PlayerMap.Model;

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
            sb.AppendLine($"Key;Id;Name;League;LeagueId;Season;SeasonId;BirthDate;BirthPlace;College;Debut;Height;Weight;Team");
            foreach (var group in groups)
            {
                foreach (var p in group)
                {
                    if (!boxScores.TryGetValue(p.Id, out var playerBs))
                        playerBs = new HashSet<BoxScore> { new BoxScore() };
                    
                    foreach (var bs in playerBs)
                    {
                        sb.AppendLine($"{group.Key};{p.Id};{p.Name};{bs.LeagueName};{bs.LeagueId};{bs.SeasonName};{bs.SeasonId};{p.BirthDate};{p.BirthPlace};{p.College};{p.Debut};{p.Height};{p.Weight};{p.Team}");    
                    }
                }
            }
            File.WriteAllText(@"C:\temp\master.players.csv", sb.ToString());
        }

        private static List<Player> GetPlayers(ZipArchiveEntry zipArchiveEntry)
        {
            var players = new List<Player>();
            using var sw = new StreamReader(zipArchiveEntry.Open());
            var headers = sw.ReadLine();
            while (!sw.EndOfStream)
            {
                var s = sw.ReadLine();
                var split = s.Split(";");
                var p = new Player
                {
                    Id = split[0],
                    Name = split[8],
                    BirthDate = DateTime.Parse(split[10]),
                    BirthPlace = split[11],
                    College = split[12],
                    Debut = DateTime.Parse(split[13]),
                    Height = int.Parse(split[15]),
                    Weight = int.Parse(split[20]),
                    Team = split[23]
                };
                p.Name = Clean(p.Name);
                players.Add(p);
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
    }
}