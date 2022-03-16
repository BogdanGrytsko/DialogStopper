using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PlayerMap.Model
{
    public class MasterPlayer
    {
        [JsonIgnore]
        public HashSet<string> PlayerIds { get; set; }
        [JsonIgnore]
        public HashSet<int> PlayerIids { get; set; }
        [JsonIgnore]
        public List<Player> Players { get; set; }
        public List<LeagueSeason> LeagueSeasons { get; set; }
        public string Name => Players.First().GetName();
        public int Count => Players.Count;
        public int DSPlayerId { get; set; }
        
        public double Correctness { get; set; }
        public string Status { get; set; }

        public MasterPlayer()
        {
            PlayerIds = new HashSet<string>();
            PlayerIids = new HashSet<int>();
            Players = new List<Player>();
            LeagueSeasons = new List<LeagueSeason>();
        }

        public override string ToString()
        {
            return $"{Name} {Count} {Status} {Correctness}";
        }

        public void AnalyzeCorrectness()
        {
            HandleCorrectness();
            HandleSeasons();
            HandleCollege();
        }

        private void HandleSeasons()
        {
            if (!LeagueSeasons.Any()) return;
            var x = (double)LeagueSeasons.Select(x => x.SeasonName).Distinct().Count() / LeagueSeasons.Count;
            if (x < 0.6)
                Status += "Often playing in 2 leagues";
        }

        private void HandleCorrectness()
        {
            var lowLimit = 10;
            var highLimit = 30;
            var cnt = Math.Min(highLimit, Count);
            cnt = Math.Max(cnt, lowLimit);
            Correctness = 100 - (cnt - lowLimit) * 100 / (highLimit - lowLimit);
            if (Correctness < 100)
                Status = "Lot's of players";
        }

        private void HandleCollege()
        {
            var nonCollegeFound = false;
            foreach (var leagueSeason in LeagueSeasons)
            {
                if (!leagueSeason.LeagueName.Contains("College"))
                    nonCollegeFound = true;
                else if (nonCollegeFound)
                {
                    Status = "College after normal league";
                    Correctness = 0;
                }
            }
        }

        public int GetDSPlayerId()
        {
            return Players.FirstOrDefault(x => x.DSPlayerId > 0)?.DSPlayerId ?? 0;
        }
    }
}