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
using Newtonsoft.Json;

namespace SpotifyAPI
{
    /// <summary>
    ///     Represents an album.
    /// </summary>
    public class Album : ResourcePromise<Album.Resource>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Album" /> class.
        /// </summary>
        /// <param name="name">The name of the album.</param>
        /// <param name="uri">The uri of the album.</param>
        internal Album(string name, string uri) : base(name, uri)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Album" /> class.
        /// </summary>
        /// <param name="partialResource">The partial resource of the album.</param>
        internal Album(Resource partialResource)
            : base(partialResource)
        {
        }

        /// <summary>
        ///     Gets a collection of genres of this <see cref="Album" />.
        /// </summary>
        public IEnumerable<string> Genres
        {
            get { return this["Genres"].Genres; }
        }

        /// <summary>
        ///     Gets a collection of <see cref="Artist">Artists</see> featured on this <see cref="Album" />.
        /// </summary>
        public IEnumerable<Artist> Artists
        {
            get { return this["Artists"].Artists.Select(r => new Artist(r)); }
        }

        /// <summary>
        ///     Gets a collection of cover art images of this <see cref="Album" />.
        /// </summary>
        public IEnumerable<ImageResource> Images
        {
            get { return this["Images"].Images; }
        }

        /// <summary>
        ///     Gets a collection of tracks of this <see cref="Album" />.
        /// </summary>
        public TrackList Tracks
        {
            get { return new TrackList(new TrackListPart(this["Tracks"].Tracks, this)); }
        }

        /// <summary>
        ///     Gets the release date of this <see cref="Album" />.
        /// </summary>
        public string ReleaseDate
        {
            get { return this["ReleaseDate"].ReleaseDate; }
        }

        /// <summary>
        ///     Gets the release date precision of this <see cref="Album" />.
        /// </summary>
        public string ReleaseDatePrecision
        {
            get { return this["ReleaseDatePrecision"].ReleaseDatePrecision; }
        }


        /// <summary>
        ///     Represents the json resource as provided by the API.
        /// </summary>
        public class Resource : BaseResource
        {
            [JsonProperty("artists")]
            public Artist.Resource[] Artists { get; set; }

            [JsonProperty("genres")]
            public string[] Genres { get; set; }

            [JsonProperty("images")]
            public ImageResource[] Images { get; set; }

            [JsonProperty("tracks")]
            public TrackListPart.Resource Tracks { get; set; }

            [JsonProperty("release_date")]
            public string ReleaseDate { get; set; }

            [JsonProperty("release_date_precision")]
            public string ReleaseDatePrecision { get; set; }
        }
    }
}