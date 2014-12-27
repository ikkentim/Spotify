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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using SpotifyAPI.Responses;

namespace SpotifyAPI
{
    public class Spotify
    {
        private readonly WebClient _webClient;
        private bool _autoUpdate;
        private int _autoUpdateInterval = 1000;
        private string _csrfToken;
        private Track _currentTrack;
        private bool _lastPlaying;
        private double _lastPlayingTime;
        private double _lastVolume;
        private string _oauthToken;

        public Spotify()
        {
            RunHelper();

            _webClient = new WebClient();
            _webClient.Headers.Add("Origin", "https://embed.spotify.com");


            Update();
        }

        public Track CurrentTrack
        {
            get { return _currentTrack; }
            set { if (value != null) Process.Start(value.URI); }
        }

        public bool IsPlaying { get; private set; }
        public bool IsShuffling { get; private set; }
        public bool IsRepeating { get; private set; }
        public bool IsPlayEnabled { get; private set; }
        public bool IsPreviousEnabled { get; private set; }
        public bool IsNextEnabled { get; private set; }
        public bool IsOnline { get; private set; }
        public bool IsAvailable { get; private set; }
        public double PlayingTime { get; private set; }
        public double Volume { get; private set; }

        public bool AutoUpdate
        {
            get { return _autoUpdate; }
            set
            {
                if (value == _autoUpdate) return;
                _autoUpdate = value;

                if (value) DoAutoUpdate();
            }
        }

        public int AutoUpdateInterval
        {
            get { return _autoUpdateInterval; }
            set
            {
                if (_autoUpdateInterval < 50) throw new ArgumentOutOfRangeException("Must be 50 or greater");
                _autoUpdateInterval = value;
            }
        }

        private void Boot()
        {
            _csrfToken = GetCFIDToken();
            _oauthToken = GetOAuthToken();
        }

        private async void DoAutoUpdate()
        {
            while (AutoUpdate)
            {
                Update();
                Thread.Sleep(AutoUpdateInterval);
            }
        }

        public event EventHandler TrackChanged;
        public event EventHandler VolumeChanged;
        public event EventHandler PlayStateChanged;
        public event EventHandler PlayingTimeChanged;
        public event EventHandler AvailabilityChanged;

        public void Update()
        {
            if ((!IsRunning || !IsHelperRunning) && IsAvailable)
            {
                IsAvailable = false;

                if (AvailabilityChanged != null)
                    AvailabilityChanged(this, EventArgs.Empty);
            }


            if (IsRunning && IsHelperRunning && !IsAvailable)
            {
                IsAvailable = true;

                if (AvailabilityChanged != null)
                    AvailabilityChanged(this, EventArgs.Empty);

                Boot();
            }

            if (IsRunning && !IsHelperRunning)
            {
                RunHelper();
            }

            if (!IsAvailable)
            {
                return;
            }

            StatusResponse status = QueryStatus();

            if (_currentTrack == null || _currentTrack.URI != status.TrackInfo.TrackResource.URI)
            {
                _currentTrack = new Track(status.TrackInfo.TrackResource.Name, status.TrackInfo.TrackResource.URI,
                    status.TrackInfo.Length,
                    new Artist(status.TrackInfo.ArtistResource.Name, status.TrackInfo.ArtistResource.URI),
                    new Album(status.TrackInfo.AlbumResource.Name, status.TrackInfo.AlbumResource.URI));

                if (TrackChanged != null)
                    TrackChanged(this, EventArgs.Empty);
            }

            IsPlaying = status.Playing;
            IsShuffling = status.Shuffle;
            IsRepeating = status.Repeat;
            IsPlayEnabled = status.PlayEnabled;
            IsPreviousEnabled = status.PrevEnabled;
            IsNextEnabled = status.NextEnabled;
            IsOnline = status.Online;

            PlayingTime = status.PlayingPosition;
            Volume = status.Volume;

            if (_lastVolume != Volume && VolumeChanged != null)
                VolumeChanged(this, EventArgs.Empty);

            if (_lastPlaying != IsPlaying && PlayStateChanged != null)
                PlayStateChanged(this, EventArgs.Empty);

            if (_lastPlayingTime != PlayingTime && PlayingTimeChanged != null)
                PlayingTimeChanged(this, EventArgs.Empty);

            _lastPlaying = IsPlaying;
            _lastVolume = Volume;
            _lastPlayingTime = PlayingTime;
        }

        public override string ToString()
        {
            return string.Format("Spotify(PlayingTime: {1}:{2:00}, CurrentTrack: {0})",
                CurrentTrack, (int) PlayingTime/60, (int) PlayingTime%60);
        }

        #region Processes

        public static bool IsRunning
        {
            get { return Process.GetProcessesByName("spotify").Length >= 1; }
        }


        public static bool IsHelperRunning
        {
            get { return Process.GetProcessesByName("SpotifyWebHelper").Length >= 1; }
        }

        public static void RunHelper()
        {
            if (!IsHelperRunning)
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                              @"\Spotify\Data\SpotifyWebHelper.exe");
        }

        public static void Run()
        {
            if (!IsRunning)
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                              @"\Spotify\spotify.exe");
        }

        #endregion

        #region Tokens

        internal string GetCFIDToken()
        {
            string raw = Query("simplecsrf/token.json", false, false).Replace(@"\", "");

            var response = JsonConvert.DeserializeObject<CFIDResponse>(raw);

            if (response == null || response.Error != null)
                throw new Exception("CFID couldn't be loaded");

            return response.Token;
        }

        internal string GetOAuthToken()
        {
            string raw;
            using (var wc = new WebClient())
            {
                raw = wc.DownloadString("http://open.spotify.com/token");
            }

            var response = JsonConvert.DeserializeObject<Dictionary<String, object>>(raw);
            return (string) response["t"];
        }

        #endregion

        #region Queries

        internal StatusResponse QueryStatus()
        {
            string response = Query("remote/status.json", true, true);

            if (string.IsNullOrEmpty(response)) return null;

            //byte[] bytes = Encoding.Default.GetBytes(response);
            //response = Encoding.UTF8.GetString(bytes);

            return JsonConvert.DeserializeObject<StatusResponse>(response);
        }

        internal string Query(string request, bool oauth, bool cfid)
        {
            string parameters = "?ref=&cors=&_=" + Timestamp.Generate();

            if (oauth) parameters += "&oauth=" + _oauthToken;
            if (cfid) parameters += "&csrf=" + _csrfToken;


            string a = @"http://localhost:4380/" + request + parameters;

            return _webClient.DownloadString(a).Replace("\\n", string.Empty);
        }

        #endregion
    }
}