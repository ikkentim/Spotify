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

namespace SpotifyAPI
{
    /// <summary>
    ///     Represents a cover art image.
    /// </summary>
    public class ImageResource
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageResource" /> class.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="url">The URL to the image.</param>
        public ImageResource(int width, int height, string url)
        {
            Width = width;
            Height = height;
            URL = url;
        }

        /// <summary>
        ///     Gets or sets the width of the image.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; set; }

        /// <summary>
        ///     Gets or sets the height of the image.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; set; }

        /// <summary>
        ///     Gets or sets the URL to the image.
        /// </summary>
        [JsonProperty("url")]
        public string URL { get; set; }
    }
}