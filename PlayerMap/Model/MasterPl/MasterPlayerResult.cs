using System.Collections.Generic;

namespace PlayerMap.Model.MasterPl
{
    public class MasterPlayerResult
    {
        public string Name { get; set; }
        public int IDSPlayerId { get; set; }
        public double Correctness { get; set; }
        public string Comment { get; set; }

        public List<LeagueSeason> PlayerInfos { get; set; }
    }
}