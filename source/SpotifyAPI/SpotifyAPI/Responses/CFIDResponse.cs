using Newtonsoft.Json;

namespace SpotifyAPI.Responses
{
    internal class CFIDResponse
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("client_version")]
        public string ClientVersion { get; set; }

        [JsonProperty("running")]
        public bool Running { get; set; }
    }
}