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

using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace SpotifyAPI
{
    public abstract class ResourcePromise<T> where T : ResourcePromise<T>.BaseResource
    {
        protected T PartialResource;
        private T _info;

        protected ResourcePromise(string name, string uri)
        {
            Name = name;
            URI = uri;
        }

        protected ResourcePromise(T partialResource)
        {
            PartialResource = partialResource;
            Name = partialResource.Name;
            URI = partialResource.URI;
        }

        public string Name { get; private set; }

        public string URI { get; private set; }

        public string OpenURL
        {
            get { return string.Format(@"http://open.spotify.com/{0}/{1}", Type, Identifier); }
        }

        public string Identifier
        {
            get { return URI.Split(':').LastOrDefault(); }
        }

        public string Type
        {
            get { return URI.Split(':').FirstOrDefault(part => part != "spotify"); }
        }

        private T Info
        {
            get
            {
                if (_info != null) return _info;

                using (var webClient = new WebClient())
                {
                    string raw =
                        webClient.DownloadString(string.Format("https://api.spotify.com/v1/{0}s/{1}", Type, Identifier));
                    return _info = JsonConvert.DeserializeObject<T>(raw);
                }
            }
            set { _info = value; }
        }

        internal T this[string key]
        {
            get
            {
                if (PartialResource == null ||
                    typeof (T).GetProperty(key).GetMethod.Invoke(PartialResource, null) == null)
                    return Info;

                return PartialResource;
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