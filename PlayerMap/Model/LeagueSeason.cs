using System;

namespace PlayerMap.Model
{
    public class LeagueSeason
    {
        public int IDSPlayerId { get; set; }
        
        public MonikerRef Player { get; set; }
        public MonikerRef League { get; set; }
        public MonikerRef Season { get; set; }
        public MonikerRef Team { get; set; }

        public override string ToString()
        {
            return $"{Season.Name} {League.Name} {Team.Name} {Player.id}";
        }
    }
}