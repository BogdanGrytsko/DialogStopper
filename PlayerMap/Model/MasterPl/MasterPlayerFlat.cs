namespace PlayerMap.Model.MasterPl
{
    public class MasterPlayerFlat
    {
        public string Name { get; set; }
        public int IDSPlayerId { get; set; }
        public double Correctness { get; set; }
        public string Comment { get; set; }
        public string JazzId { get; set; }
        
        public BasicMonikerRef Player { get; set; }
        public MonikerRef League { get; set; }
        public BasicMonikerRef Season { get; set; }
        public MonikerRef Team { get; set; }
    }
}