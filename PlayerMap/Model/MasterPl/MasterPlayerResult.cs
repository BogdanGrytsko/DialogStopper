using System.Collections.Generic;
using PlayerMap.Model.Scrape;

namespace PlayerMap.Model.MasterPl
{
    public class MasterPlayerResult
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public int IDSPlayerId { get; set; }
        public double Correctness { get; set; }
        public string Comment { get; set; }
        public string JazzId { get; set; }
        
        public HashSet<uint> PlayerIids { get; set; }
        public List<LeagueSeason> PlayerInfos { get; set; }
        public NbaPlayer NbaPlayer { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}