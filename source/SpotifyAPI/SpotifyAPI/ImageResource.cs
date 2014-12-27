using Newtonsoft.Json;

namespace SpotifyAPI
{
    public class ImageResource
    {
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("url")]
        public string URL { get; set; }

        public ImageResource(int width, int height, string url)
        {
            Width = width;
            Height = height;
            URL = url;
        }
    }
}