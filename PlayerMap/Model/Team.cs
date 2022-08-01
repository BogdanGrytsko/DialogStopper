using Newtonsoft.Json;

namespace PlayerMap.Model
{
    public class Team
    {
        [JsonIgnore]
        public string MongoId { get; set; }
        public string Name { get; set; }
    }
}