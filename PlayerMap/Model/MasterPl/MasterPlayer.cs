using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PlayerMap.Model.MasterPl
{
    public class MasterPlayer
    {
        public HashSet<string> PlayerIds { get; set; }
        public HashSet<int> PlayerIids { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public List<Player> Players { get; set; }
        [JsonProperty("PlayerInfos")]
        public List<LeagueSeason> LeagueSeasons { get; set; }
        public string Name { get; set; }

        public string Key { get; set; }
        public int Count => Players.Count;
        public int IDSPlayerId { get; set; }
        
        public double Correctness { get; set; }
        public string Comment { get; private set; }
        public List<string> Comments { get; set; }

        public MasterPlayer()
        {
            PlayerIds = new HashSet<string>();
            PlayerIids = new HashSet<int>();
            Players = new List<Player>();
            LeagueSeasons = new List<LeagueSeason>();
            Comments = new List<string>();
        }

        public override string ToString()
        {
            return $"{Name} {Count} {Comment} {Correctness}";
        }

        public void SetName()
        {
            Name = Players.First().GetName();
        }

        public void AnalyzeCorrectness()
        {
            HandleCollegeYears();
            HandleCorrectness();
            HandleSeasons();
            HandleLeagueSequence();
            Comment = string.Join("; ", Comments);
        }

        public void SetData()
        {
            SetName();
            FilterOutInternalLeagues();
            FillInSeasonIids();
        }

        private void FillInSeasonIids()
        {
            foreach (var leagueSeason in LeagueSeasons)
            {
                leagueSeason.Season.iid = uint.Parse(leagueSeason.Season.Name.Split("-").First());
            }
        }
        
        public void FilterOutInternalLeagues()
        {
            LeagueSeasons = LeagueSeasons.Where(x => !Leagues.InternalLeagues.ContainsKey(x.League.id)).ToList();
        }
        
        private void HandleCollegeYears()
        {
            var colleges = LeagueSeasons.Where(x => Leagues.InAnyCollege(x.League.id)).ToList();
            if (colleges.Any())
            {
                var yearMin = colleges.First().Season.iid;
                var yearMax = colleges.Last().Season.iid;
                if (yearMax - yearMin > 5)
                {
                    Comments.Add("Spent more than 5 years in College");
                }
            }
        }

        private void HandleSeasons()
        {
            if (!LeagueSeasons.Any()) return;
            var differentSeasons = LeagueSeasons.Select(x => x.Season.Name).Distinct().Count();
            var x = (double)differentSeasons / LeagueSeasons.Count;
            if (x < 0.5)
                Comments.Add("Often playing in 2 leagues during same season");
        }

        private void HandleCorrectness()
        {
            var lowLimit = 15;
            var highLimit = 30;
            var cnt = Math.Min(highLimit, Count);
            cnt = Math.Max(cnt, lowLimit);
            Correctness = 100 - (cnt - lowLimit) * 100 / (highLimit - lowLimit);
            if (Correctness < 100)
                Comments.Add($"More than {lowLimit} players mapped to same master player");
        }

        private void HandleLeagueSequence()
        {
            int maxLeague = -1;
            foreach (var leagueSeason in LeagueSeasons)
            {
                var newMaxLeague = Leagues.GetLeagueValue(leagueSeason.League.id);
                if (newMaxLeague < maxLeague)
                {
                    //SPLIT!
                    Comments.Add(Leagues.GetComment(maxLeague, newMaxLeague));
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