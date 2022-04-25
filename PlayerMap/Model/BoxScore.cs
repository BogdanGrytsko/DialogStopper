using System;

namespace PlayerMap.Model
{
    public class BoxScore
    {
        public string LeagueId { get; set; }
        public string LeagueName { get; set; }
        
        public string SeasonId { get; set; }
        public string SeasonName { get; set; }
        
        public string PlayerId { get; set; }

        protected bool Equals(BoxScore other)
        {
            return LeagueId == other.LeagueId && SeasonId == other.SeasonId && PlayerId == other.PlayerId;
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
            return HashCode.Combine(LeagueId, SeasonId, PlayerId);
        }
    }
}