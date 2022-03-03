using System;

namespace PlayerMap.Model
{
    public class BoxScore
    {
        public string PlayerId { get; set; }
        
        public string LeagueId { get; set; }
        public string LeagueName { get; set; }
        
        public string SeasonId { get; set; }
        public string SeasonName { get; set; }

        protected bool Equals(BoxScore other)
        {
            return PlayerId == other.PlayerId && LeagueId == other.LeagueId && SeasonId == other.SeasonId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BoxScore)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PlayerId, LeagueId, SeasonId);
        }
    }
}