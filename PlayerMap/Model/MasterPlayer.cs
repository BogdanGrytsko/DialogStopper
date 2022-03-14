using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PlayerMap.Model
{
    public class MasterPlayer
    {
        public HashSet<string> PlayerIds { get; set; }
        public HashSet<int> PlayerIids { get; set; }
        [JsonIgnore]
        public List<Player> Players { get; set; }
        public List<LeagueSeason> LeagueSeasons { get; set; }
        
        public double Correctness { get; set; }

        public MasterPlayer()
        {
            PlayerIds = new HashSet<string>();
            PlayerIids = new HashSet<int>();
            Players = new List<Player>();
            LeagueSeasons = new List<LeagueSeason>();
        }

        public override string ToString()
        {
            return $"{Players.First().GetName()} {PlayerIids.Count}";
        }
    }
}