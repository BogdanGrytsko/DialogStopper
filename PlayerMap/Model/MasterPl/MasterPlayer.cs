using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PlayerMap.Model.MasterPl
{
    public class MasterPlayer
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public HashSet<string> PlayerIds { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public HashSet<int> PlayerIids { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public List<Player> Players { get; set; }
        [JsonProperty("PlayerInfos")]
        public List<LeagueSeason> LeagueSeasons { get; set; }
        public string Name => Players.First().GetName();
        public string Key { get; set; }
        public int Count => Players.Count;
        public int IDSPlayerId { get; set; }
        
        public double Correctness { get; set; }
        public string Comment { get; set; }

        public MasterPlayer()
        {
            PlayerIds = new HashSet<string>();
            PlayerIids = new HashSet<int>();
            Players = new List<Player>();
            LeagueSeasons = new List<LeagueSeason>();
        }

        public override string ToString()
        {
            return $"{Name} {Count} {Comment} {Correctness}";
        }

        public void AnalyzeCorrectness()
        {
            FilterOutInternalLeagues();
            HandleCorrectness();
            HandleSeasons();
            HandleLeagueSequence();
        }

        private void FilterOutInternalLeagues()
        {
            LeagueSeasons = LeagueSeasons.Where(x => !Leagues.InternalLeagues.ContainsKey(x.League.id)).ToList();
        }

        private void HandleSeasons()
        {
            if (!LeagueSeasons.Any()) return;
            var x = (double)LeagueSeasons.Select(x => x.Season.Name).Distinct().Count() / LeagueSeasons.Count;
            if (x < 0.6)
                Comment += "Often playing in 2 leagues";
        }

        private void HandleCorrectness()
        {
            var lowLimit = 15;
            var highLimit = 30;
            var cnt = Math.Min(highLimit, Count);
            cnt = Math.Max(cnt, lowLimit);
            Correctness = 100 - (cnt - lowLimit) * 100 / (highLimit - lowLimit);
            if (Correctness < 100)
                Comment = "Lot's of players";
        }

        private void HandleLeagueSequence()
        {
            int maxLeague = -1;
            foreach (var leagueSeason in LeagueSeasons)
            {
                var newMaxLeague = Leagues.GetLeagueValue(leagueSeason.League.id);
                if (newMaxLeague < maxLeague)
                {
                    Comment = Leagues.GetComment(maxLeague, newMaxLeague);
                    Correctness = 0;
                    break;
                }
                maxLeague = newMaxLeague;
            }
        }

        public int GetDSPlayerId()
        {
            return Players.FirstOrDefault(x => x.DSPlayerId > 0)?.DSPlayerId ?? 0;
        }
    }
}