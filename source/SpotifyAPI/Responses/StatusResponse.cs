// SpotifyAPI
// Copyright (C) 2014 Tim Potze
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>

using Newtonsoft.Json;

namespace SpotifyAPI.Responses
{
    internal class StatusResponse
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("client_version")]
        public string ClientVersion { get; set; }

        [JsonProperty("running")]
        public bool Running { get; set; }

        [JsonProperty("playing")]
        public bool Playing { get; set; }

        [JsonProperty("shuffle")]
        public bool Shuffle { get; set; }

        [JsonProperty("repeat")]
        public bool Repeat { get; set; }

        [JsonProperty("play_enabled")]
        public bool PlayEnabled { get; set; }

        [JsonProperty("prev_enabled")]
        public bool PrevEnabled { get; set; }

        [JsonProperty("next_enabled")]
        public bool NextEnabled { get; set; }

        [JsonProperty("track")]
        public TrackInfo TrackInfo { get; set; }

        [JsonProperty("playing_position")]
        public double PlayingPosition { get; set; }

        [JsonProperty("server_time")]
        public int ServerTime { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }

        [JsonProperty("online")]
        public bool Online { get; set; }
    }
}