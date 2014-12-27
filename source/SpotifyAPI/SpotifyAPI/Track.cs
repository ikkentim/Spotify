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
    public class Track : ResourcePromise<Track.Resource>
    {
        private Artist _artist;

        internal Track(string name, string uri, int length, Artist artist, Album album)
            : base(name, uri)
        {
            Length = length*1000;
            Artist = artist;
            Album = album;
        }

        internal Track(Resource partialResource)
            : base(partialResource)
        {
            Length = this["Duration"].Duration;
        }

        internal Track(Resource partialResource, Album album)
            : this(partialResource)
        {
            Album = album;
        }

        public Artist Artist
        {
            get { return _artist ?? Artists.FirstOrDefault(); }
            private set { _artist = value; }
        }

        public Album Album { get; private set; }

        public int Length { get; private set; }

        public ImageResource[] AlbumImages
        {
            get { return this["Album"].Album.Images; }
        }

        public IEnumerable<Artist> Artists
        {
            get { return this["Artists"].Artists.Select(r => new Artist(r)); }
        }

        public override string ToString()
        {
            return string.Format("{1} - {0} ({2}) ({3}:{4:00})", Name, Artist, Album, (Length/1000)/60, (Length/1000)%60);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Track) obj);
        }

        protected bool Equals(Track other)
        {
            return other != null && other.Identifier == Identifier;
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

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