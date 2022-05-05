using Newtonsoft.Json;

namespace PlayerMap.Jazz
{
    public class JazzDto
    {
        [JsonProperty("source_player_id")]
        public string SourcePlayerId { get; set; }
        [JsonProperty("source_player_full_name")]
        public string SourcePlayerName { get; set; }
        [JsonProperty("target_player_id")]
        public string TargetPlayerId { get; set; }
        [JsonProperty("target_player_full_name")]
        public string TargetPlayerName { get; set; }
    }
}