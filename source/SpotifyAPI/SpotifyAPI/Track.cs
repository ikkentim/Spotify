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
    ///     Represents a track.
    /// </summary>
    public class Track : ResourcePromise<Track.Resource>
    {
        #region Fields

        private Artist _artist; // contains artist when track is only partially loaded.

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Track" /> class.
        /// </summary>
        /// <param name="name">The name of the track.</param>
        /// <param name="uri">The uri of the track.</param>
        /// <param name="length">The length of the track in seconds.</param>
        /// <param name="artist">The <see cref="Artist" /> of the track.</param>
        /// <param name="album">The <see cref="Album" /> of the track.</param>
        internal Track(string name, string uri, int length, Artist artist, Album album)
            : base(name, uri)
        {
            Length = length*1000;
            Artist = artist;
            Album = album;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Track" /> class.
        /// </summary>
        /// <param name="partialResource">The partial resource of the track.</param>
        internal Track(Resource partialResource)
            : base(partialResource)
        {
            Length = this["Duration"].Duration;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Track" /> class.
        /// </summary>
        /// <param name="partialResource">The partial resource of the track.</param>
        /// <param name="album">The <see cref="Album" /> of the track.</param>
        internal Track(Resource partialResource, Album album)
            : this(partialResource)
        {
            Album = album;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the <see cref="Artist" /> of this <see cref="Track" />.
        /// </summary>
        public Artist Artist
        {
            get { return _artist ?? Artists.FirstOrDefault(); }
            private set { _artist = value; }
        }

        /// <summary>
        ///     Gets the <see cref="Album" /> of this <see cref="Track" />.
        /// </summary>
        public Album Album { get; private set; }

        /// <summary>
        ///     Gets the length of this <see cref="Track" /> in MS.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        ///     Gets this <see cref="Track" />'s <see cref="Album" />'s cover images.
        /// </summary>
        public ImageResource[] AlbumImages
        {
            get { return this["Album"].Album.Images; }
        }

        /// <summary>
        ///     Gets the artists featured in this <see cref="Track" />.
        /// </summary>
        public IEnumerable<Artist> Artists
        {
            get { return this["Artists"].Artists.Select(r => new Artist(r)); }
        }

        #endregion

        #region Object implementation

        /// <summary>
        ///     Returns a string that represents this <see cref="Track" />.
        /// </summary>
        /// <returns>
        ///     A string that represents this <see cref="Track" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{1} - {0} ({2}) ({3}:{4:00})", Name, Artist, Album, (Length/1000)/60, (Length/1000)%60);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        ///     <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Track) obj);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Track" /> is equal to the current <see cref="Track" />.
        /// </summary>
        /// <param name="other">The <see cref="Track" /> to compare with the current <see cref="Track" />.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        protected bool Equals(Track other)
        {
            return other != null && other.Identifier == Identifier;
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///     A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        #endregion

        /// <summary>
        ///     Represents the json resource as provided by the API.
        /// </summary>
        public class Resource : BaseResource
        {
            [JsonProperty("album")]
            public Album.Resource Album { get; set; }

            [JsonProperty("artists")]
            public Artist.Resource[] Artists { get; set; }

            [JsonProperty("duration_ms")]
            public int Duration { get; set; }
        }
    }
}