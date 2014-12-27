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
    /// <summary>
    ///     Represents a resource.
    /// </summary>
    /// <typeparam name="T">The base resource of the resource.</typeparam>
    public abstract class ResourcePromise<T> where T : ResourcePromise<T>.BaseResource
    {
        protected T PartialResource; // Contains partial resource information
        private T _info; // Contains full resource information

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResourcePromise`1" /> class.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        /// <param name="uri">The uri of the resource.</param>
        protected ResourcePromise(string name, string uri)
        {
            Name = name;
            URI = uri;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResourcePromise`1" /> class.
        /// </summary>
        /// <param name="partialResource">The partial resource of this resource.</param>
        protected ResourcePromise(T partialResource)
        {
            PartialResource = partialResource;
            Name = partialResource.Name;
            URI = partialResource.URI;
        }

        /// <summary>
        ///     Gets the name of this resource.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the URI of this resource.
        /// </summary>
        public string URI { get; private set; }

        /// <summary>
        ///     Gets the url to show this resource in a browser.
        /// </summary>
        public string OpenURL
        {
            get { return string.Format(@"http://open.spotify.com/{0}/{1}", Type, Identifier); }
        }

        /// <summary>
        ///     Gets the identifier of this resource.
        /// </summary>
        public string Identifier
        {
            get { return URI.Split(':').LastOrDefault(); }
        }

        /// <summary>
        ///     Gets the type of this resource.
        /// </summary>
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

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     Represents the json resource as provided by the API.
        /// </summary>
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