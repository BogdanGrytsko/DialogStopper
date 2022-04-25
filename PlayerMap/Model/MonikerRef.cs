namespace PlayerMap.Model
{
    public class MonikerRef
    {
        public MonikerRef(string id, string name)
        {
            this.id = id;
            this.Name = name;
        }

        public string id { get; set; }
        public uint iid { get; set; }
        
        public string Name { get; set; }

        public string Abbr { get; set; }
    }
}