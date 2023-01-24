using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerMap.BasketballReference.Model
{
    public class NBAUpdatePlayerCareer
    {
        public string Url { get; set; }
        public string PlayerName { get; set; }

        public List<CareerItem> Items { get; set; }
    }

    public class CareerItem
    {
        public string MongoLeagueId { get; set; }
        public  string LeagueName { get; set; }
        public  string MongoSeasonId { get; set; }
        public  string SeasonName { get; set; }
        public string MongoTeamId { get; set; }
        public  string TeamName { get; set; }
        public  string MongoPlayerId { get; set; }
    }

}
