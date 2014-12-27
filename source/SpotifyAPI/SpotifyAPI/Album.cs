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
    public class Album : ResourcePromise<Album.Resource>
    {
        public Album(string name, string uri) : base(name, uri)
        {
        }

        public Album(Resource partialResource)
            : base(partialResource)
        {
        }

        public IEnumerable<string> Genres
        {
            get { return this["Genres"].Genres; }
        }

        public IEnumerable<Artist> Artists
        {
            get { return this["Artists"].Artists.Select(r => new Artist(r)); }
        }

        public IEnumerable<ImageResource> Images
        {
            get { return this["Images"].Images; }
        }

        public TrackList Tracks
        {
            get { return new TrackList(new TracksListPart(this["Tracks"].Tracks, this)); }
        }

        public string ReleaseDate
        {
            get { return this["ReleaseDate"].ReleaseDate; }
        }

        public string ReleaseDatePrecision
        {
            get { return this["ReleaseDatePrecision"].ReleaseDatePrecision; }
        }


        public class Resource : BaseResource
        {
            [JsonProperty("artists")]
            public Artist.Resource[] Artists { get; set; }

            [JsonProperty("genres")]
            public string[] Genres { get; set; }

            [JsonProperty("images")]
            public ImageResource[] Images { get; set; }

            [JsonProperty("tracks")]
            public TracksListPart.Resource Tracks { get; set; }

            [JsonProperty("release_date")]
            public string ReleaseDate { get; set; }

            [JsonProperty("release_date_precision")]
            public string ReleaseDatePrecision { get; set; }
        }
    }
}