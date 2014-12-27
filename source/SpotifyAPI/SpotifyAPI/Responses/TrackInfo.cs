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

    internal class TrackResource
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("uri")]
        public string URI { get; set; }

        [JsonProperty("location")]
        public TrackResourceLocation Location { get; set; }
    }

    internal class TrackResourceLocation
    {
        [JsonProperty("og")]
        public string Og { get; set; }
    }
}
