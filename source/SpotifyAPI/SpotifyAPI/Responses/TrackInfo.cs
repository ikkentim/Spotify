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
    internal class TrackInfo
    {
        [JsonProperty("track_resource")]
        public TrackResource TrackResource { get; set; }

        [JsonProperty("artist_resource")]
        public TrackResource ArtistResource { get; set; }

        [JsonProperty("album_resource")]
        public TrackResource AlbumResource { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("track_type")]
        public string TrackType { get; set; }
    }
}