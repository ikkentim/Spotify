using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SpotifyAPI.Responses;

namespace SpotifyAPI
{
    public class Track : ResourcePromise<Track.Resource>
    {
        public Track(string name, string uri, int length, Artist artist, Album album) : base(name, uri)
        {
            Length = length;
            Artist = artist;
            Album = album;
        }

        public Track(Resource partialResource) : base(partialResource)
        {
            
        }

        public Artist Artist { get; private set; }

        public Album Album { get; private set; }

        public int Length { get; private set; }

        public override string ToString()
        {
            return string.Format("{1} - {0} ({2}) ({3}:{4:00})", Name, Artist, Album, Length / 60, Length % 60);
        }

        public ImageResource[] AlbumImages
        {
            get { return this["Album"].Album.Images; }
        }

        public IEnumerable<Artist> Artists
        {
            get { return this["Artists"].Artists.Select(r => new Artist(r)); }
        }

        public class Resource : BaseResource
        {
            [JsonProperty("album")]
            public Album.Resource Album { get; set; }

            [JsonProperty("artists")]
            public Artist.Resource[] Artists { get; set; }
        }
    }
}
