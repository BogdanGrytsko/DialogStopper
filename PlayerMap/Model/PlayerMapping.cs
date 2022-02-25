using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestTask.PlayerMap
{
    public class PlayerMapping
    {
        public static void Run()
        {
            var players = GetPlayers();
            var boxScores = GetBoxScoresMap();

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

        private static List<Player> GetPlayers()
        {
            var playerCsv = File.ReadAllLines(@"C:\temp\sport.players.csv");
            var players = new List<Player>();
            foreach (var s in playerCsv.Skip(1))
            {
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
                players.Add(p);
            }

            players = players.Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList();
            return players;
        }
        
        private static Dictionary<string, HashSet<BoxScore>> GetBoxScoresMap()
        {
            var boxScoresCsv = File.ReadAllLines(@"C:\temp\basketball.boxscores.players_mapinfo.csv");
            var boxScores = new List<BoxScore>();
            foreach (var s in boxScoresCsv.Skip(1))
            {
                var split = s.Split(";");
                var score = new BoxScore
                {
                    LeagueId = split[1],
                    LeagueName = split[2],
                    SeasonId = split[3],
                    SeasonName = split[4],
                    PlayerId = split[5]
                };
                boxScores.Add(score);
            }

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