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

using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace SpotifyAPI
{
    internal class TracksListPart : ResourcePromise<TracksListPart.Resource>
    {
        private readonly Album _album;
        private TracksListPart _next;
        private TracksListPart _previous;

        public TracksListPart(string url, Album album) : base(null, null)
        {
            _album = album;
            using (var webClient = new WebClient())
            {
                string raw = webClient.DownloadString(url);
                PartialResource = JsonConvert.DeserializeObject<Resource>(raw);
            }
        }

        public TracksListPart(Resource partialResource, Album album)
            : base(partialResource)
        {
            _album = album;
        }

        public IEnumerable<Track> Tracks
        {
            get { return this["Items"].Items.Select(r => new Track(r, _album)); }
        }

        public TracksListPart Next
        {
            get
            {
                return _next ??
                       (this["Next"].Next == null ? null : _next = new TracksListPart(this["Next"].Next, _album));
            }
        }

        public TracksListPart Previous
        {
            get
            {
                return _previous ??
                       (this["Previous"].Previous == null
                           ? null
                           : _previous = new TracksListPart(this["Previous"].Previous, _album));
            }
        }

        public int Limit
        {
            get { return this["Limit"].Limit; }
        }

        public int Offset
        {
            get { return this["Offset"].Offset; }
        }

        public int Total
        {
            get { return this["Total"].Total; }
        }

        public class Resource : BaseResource
        {
            [JsonProperty("items")]
            public Track.Resource[] Items { get; set; }

            [JsonProperty("next")]
            public string Next { get; set; }

            [JsonProperty("previous")]
            public string Previous { get; set; }

            [JsonProperty("limit")]
            public int Limit { get; set; }

            [JsonProperty("offset")]
            public int Offset { get; set; }

            [JsonProperty("total")]
            public int Total { get; set; }
        }
    }
}