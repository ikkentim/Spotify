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
    /// <summary>
    ///     Represents a part of an album's track list.
    /// </summary>
    public class TrackListPart : ResourcePromise<TrackListPart.Resource>
    {
        #region Fields

        private readonly Album _album; // The album of this list.
        private TrackListPart _next; // Cache of next part.
        private TrackListPart _previous; // Cache of previous part.

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TrackListPart" /> class.
        /// </summary>
        /// <param name="url">The url to the list.</param>
        /// <param name="album">The album of the list.</param>
        internal TrackListPart(string url, Album album) : base(null, null)
        {
            _album = album;
            using (var webClient = new WebClient())
            {
                string raw = webClient.DownloadString(url);
                PartialResource = JsonConvert.DeserializeObject<Resource>(raw);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TrackListPart" /> class.
        /// </summary>
        /// <param name="partialResource">The partial resource of the list.</param>
        /// <param name="album">The album of the list.</param>
        internal TrackListPart(Resource partialResource, Album album)
            : base(partialResource)
        {
            _album = album;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the tracks in this <see cref="TrackListPart" />.
        /// </summary>
        public IEnumerable<Track> Tracks
        {
            get { return this["Items"].Items.Select(r => new Track(r, _album)); }
        }

        /// <summary>
        ///     Gets the next <see cref="TrackListPart" />.
        /// </summary>
        public TrackListPart Next
        {
            get
            {
                return _next ??
                       (this["Next"].Next == null ? null : _next = new TrackListPart(this["Next"].Next, _album));
            }
        }

        /// <summary>
        ///     Gets the previous <see cref="TrackListPart" />.
        /// </summary>
        public TrackListPart Previous
        {
            get
            {
                return _previous ??
                       (this["Previous"].Previous == null
                           ? null
                           : _previous = new TrackListPart(this["Previous"].Previous, _album));
            }
        }

        /// <summary>
        ///     Gets the offset from the first track to this <see cref="TrackListPart" />.
        /// </summary>
        public int Offset
        {
            get { return this["Offset"].Offset; }
        }

        /// <summary>
        ///     Gets the total number of tracks in the list this <see cref="TrackListPart" /> is part of.
        /// </summary>
        public int Total
        {
            get { return this["Total"].Total; }
        }

        #endregion

        /// <summary>
        ///     Represents the json resource as provided by the API.
        /// </summary>
        public class Resource : BaseResource
        {
            [JsonProperty("items")]
            public Track.Resource[] Items { get; set; }

            [JsonProperty("next")]
            public string Next { get; set; }

            [JsonProperty("previous")]
            public string Previous { get; set; }

            [JsonProperty("offset")]
            public int Offset { get; set; }

            [JsonProperty("total")]
            public int Total { get; set; }
        }
    }
}