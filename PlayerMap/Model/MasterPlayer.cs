using System.Collections.Generic;

namespace PlayerMap.Model
{
    public class MasterPlayer
    {
        public HashSet<int> PlayerIds { get; set; }

        public List<Player> Players { get; set; }

        public MasterPlayer()
        {
            PlayerIds = new HashSet<int>();
            Players = new List<Player>();
        }
    }
}