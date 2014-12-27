using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace SpotifyAPI
{
    public abstract class ResourcePromise<T> where T : ResourcePromise<T>.BaseResource
    {
        private readonly T _partialResource;

        public string Name { get; private set; }

        public string URI { get; private set; }

        public string OpenURL
        {
            get {  return string.Format(@"http://open.spotify.com/{0}/{1}", Type, Identifier); }
        }

        public string Identifier
        {
            get { return URI.Split(':').LastOrDefault(); }
        }

        public string Type
        {
            get { return URI.Split(':').FirstOrDefault(part => part != "spotify"); }
        }

        protected ResourcePromise(string name, string uri)
        {
            Name = name;
            URI = uri;
        }

        protected ResourcePromise(T partialResource)
        {
            _partialResource = partialResource;
            Name = partialResource.Name;
            URI = partialResource.URI;
        }

        private T _info;
        private T Info
        {
            get
            {
                if (_info != null) return _info;

                using (var webClient = new WebClient())
                {
                    var raw =
                        webClient.DownloadString(string.Format("https://api.spotify.com/v1/{0}s/{1}", Type, Identifier));
                    return _info = JsonConvert.DeserializeObject<T>(raw);
                }
            }
            set
            {
                _info = value;
            }
        }

        internal T this[string key]
        {
            get
            {
                if (_partialResource == null ||
                    typeof (T).GetProperty(key).GetMethod.Invoke(_partialResource, null) == null)
                    return Info;

                return _partialResource;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public abstract class BaseResource
        {
            [JsonProperty("href")]
            public string Href { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("uri")]
            public string URI { get; set; }
        }
    }
}