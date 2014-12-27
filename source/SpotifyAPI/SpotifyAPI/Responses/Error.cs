using Newtonsoft.Json;

namespace SpotifyAPI.Responses
{
    internal class Error
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}