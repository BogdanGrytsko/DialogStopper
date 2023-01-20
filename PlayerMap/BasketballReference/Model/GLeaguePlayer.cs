using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerMap.BasketballReference.Model
{
    public class GLeaguePlayer
    {
        public string Name { get; set; }
        public string PlayerUrl { get; set; }
        public string BBRefTeamId { get; set; }
        public string MongoTeamId { get; set; }
        public int BBRedSeasonId { get; set; }
        public string MongoSeasonId { get; set; }
    }
}
