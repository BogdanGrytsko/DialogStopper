using System.Collections.Generic;
using System.Linq;

namespace PlayerMap.Model
{
    public class MasterPlayer
    {
        public HashSet<string> PlayerIds { get; set; }
        public HashSet<int> PlayerIids { get; set; }
        public List<Player> Players { get; set; }
        public List<LeagueSeason> LeagueSeasons { get; set; }

        public MasterPlayer()
        {
            PlayerIds = new HashSet<string>();
            PlayerIids = new HashSet<int>();
            Players = new List<Player>();
            LeagueSeasons = new List<LeagueSeason>();
        }

        public override string ToString()
        {
            if (PlayerIds.Any())
                return $"{Players.First().Name} {PlayerIds.Count}";
            return $"{PlayerIids.Count}";
        }
    }
}