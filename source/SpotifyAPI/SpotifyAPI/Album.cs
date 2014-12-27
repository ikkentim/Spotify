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

            [JsonProperty("release_date")]
            public string ReleaseDate { get; set; }

            [JsonProperty("release_date_precision")]
            public string ReleaseDatePrecision { get; set; }
        }
    }
}