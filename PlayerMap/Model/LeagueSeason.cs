using System;

namespace PlayerMap.Model
{
    public class LeagueSeason
    {
        public int IDSPlayerId { get; set; }
        
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        
        public string LeagueId { get; set; }
        public string LeagueName { get; set; }
        
        public string SeasonId { get; set; }
        public string SeasonName { get; set; }
        public string TeamId { get; set; }
        public string TeamName { get; set; }

        protected bool Equals(LeagueSeason other)
        {
            return PlayerId == other.PlayerId && LeagueId == other.LeagueId && SeasonId == other.SeasonId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LeagueSeason)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PlayerId, LeagueId, SeasonId);
        }

        public override string ToString()
        {
            return $"{SeasonName} {LeagueName} {TeamName} {PlayerId}";
        }
    }
}