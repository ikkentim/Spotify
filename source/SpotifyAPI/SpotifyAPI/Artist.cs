using System.Collections.Generic;
using Newtonsoft.Json;

namespace SpotifyAPI
{
    public class Artist : ResourcePromise<Artist.Resource>
    {
        public Artist(string name, string uri) : base(name, uri)
        {
        }
        public Artist(Resource partialResource)
            : base(partialResource)
        {
            
        }

        public IEnumerable<string> Genres
        {
            get { return this["Genres"].Genres; }
        }

        public IEnumerable<ImageResource> Images
        {
            get { return this["Images"].Images; }
        } 

        public class Resource : BaseResource
        {
            [JsonProperty("genres")]
            public string[] Genres { get; set; }

            [JsonProperty("images")]
            public ImageResource[] Images { get; set; }
        }
    }
}