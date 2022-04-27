using System;

namespace PlayerMap.Model
{
    public class LeagueSeason
    {
        public int IDSPlayerId { get; set; }
        
        public BasicMonikerRef Player { get; set; }
        public MonikerRef League { get; set; }
        public BasicMonikerRef Season { get; set; }
        public MonikerRef Team { get; set; }

        public override string ToString()
        {
            return $"{Season.Name} {League.Name} {Team.Name} {Player.id}";
        }
    }
}