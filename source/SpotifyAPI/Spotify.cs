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
    /// <summary>
    ///     Represents a spotify client.
    /// </summary>
    public class Spotify
    {
        #region Fields

        private readonly WebClient _webClient; // web client used for fetching tokens and status.
        private bool _autoUpdate; // contains auto update toggle.
        private int _autoUpdateInterval = 1000; // contains auto update rate.
        private string _csrfToken; // contains csrf token.
        private Track _currentTrack; // contains current track.
        private bool _lastPlaying; // contains last playing state.
        private double _lastPlayingTime; // contains last playing time.
        private double _lastVolume; // contains last volume.
        private string _oauthToken; // contains oauth token.

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="Spotify" /> class.
        /// </summary>
        public Spotify()
        {
            // Ensure helper is running
            RunHelper();

            _webClient = new WebClient();
            _webClient.Headers.Add("Origin", "https://embed.spotify.com");

            // Setup and fetch initial update
            Update();
        }

        /// <summary>
        ///     Gets or sets the playing <see cref="Track" />.
        /// </summary>
        public Track CurrentTrack
        {
            get { return _currentTrack; }
            set { if (value != null) Process.Start(value.URI); }
        }

        /// <summary>
        ///     Gets whether <see cref="Spotify" /> is currently playing.
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        ///     Gets whether <see cref="Spotify" /> is currently shuffling.
        /// </summary>
        public bool IsShuffling { get; private set; }

        /// <summary>
        ///     Gets whether <see cref="Spotify" /> is currently repeating.
        /// </summary>
        public bool IsRepeating { get; private set; }

        /// <summary>
        ///     Gets whether <see cref="Spotify" /> is able to start playing.
        /// </summary>
        public bool IsPlayEnabled { get; private set; }

        /// <summary>
        ///     Gets whether <see cref="Spotify" /> is able to go back to the previous track.
        /// </summary>
        public bool IsPreviousEnabled { get; private set; }

        /// <summary>
        ///     Gets whether <see cref="Spotify" /> is able to skip to the next track.
        /// </summary>
        public bool IsNextEnabled { get; private set; }

        /// <summary>
        ///     Gets whether <see cref="Spotify" /> is currently connected to the network.
        /// </summary>
        public bool IsOnline { get; private set; }

        /// <summary>
        ///     Gets whether <see cref="Spotify" /> is currently running and available.
        /// </summary>
        public bool IsAvailable { get; private set; }

        /// <summary>
        ///     Gets whether <see cref="Spotify" /> is currently playing time in seconds.
        /// </summary>
        public double PlayingTime { get; private set; }

        /// <summary>
        ///     Gets <see cref="Spotify" />'s current volume.
        /// </summary>
        public double Volume { get; private set; }

        /// <summary>
        ///     Gets or sets whether <see cref="Update" /> should be called automatically at an interval.
        /// </summary>
        /// <remarks>
        ///     The interval can be set with <see cref="AutoUpdateInterval" />
        /// </remarks>
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

        /// <summary>
        ///     Gets or sets the update interval.
        /// </summary>
        public int AutoUpdateInterval
        {
            get { return _autoUpdateInterval; }
            set
            {
                if (_autoUpdateInterval < 50) throw new ArgumentOutOfRangeException("Must be 50 or greater");
                _autoUpdateInterval = value;
            }
        }

        /// <summary>
        ///     Fired once <see cref="CurrentTrack" /> changes.
        /// </summary>
        public event EventHandler TrackChanged;

        /// <summary>
        ///     Fired once <see cref="Volume" /> changes.
        /// </summary>
        public event EventHandler VolumeChanged;

        /// <summary>
        ///     Fired once <see cref="IsPlaying" /> changes.
        /// </summary>
        public event EventHandler PlayStateChanged;

        /// <summary>
        ///     Fired once <see cref="PlayingTime" /> changes.
        /// </summary>
        public event EventHandler PlayingTimeChanged;

        /// <summary>
        ///     Fired once <see cref="IsAvailable" /> changes.
        /// </summary>
        public event EventHandler AvailabilityChanged;

        private void Boot()
        {
            // Fetch tokens.
            _csrfToken = GetCFIDToken();
            _oauthToken = GetOAuthToken();
        }

        private async void DoAutoUpdate()
        {
            // Keep updating.
            while (AutoUpdate)
            {
                Update();
                Thread.Sleep(AutoUpdateInterval);
            }
        }

        public void Update()
        {
            // If not running, but known to be available, notify.
            if ((!IsRunning || !IsHelperRunning) && IsAvailable)
            {
                IsAvailable = false;

                if (AvailabilityChanged != null)
                    AvailabilityChanged(this, EventArgs.Empty);
            }

            // If running, but known not te be available, notify and boot.
            if (IsRunning && IsHelperRunning && !IsAvailable)
            {
                IsAvailable = true;

                if (AvailabilityChanged != null)
                    AvailabilityChanged(this, EventArgs.Empty);

                Boot();
            }

            // If running but helper is not running, boot and skip update.
            if (IsRunning && !IsHelperRunning)
            {
                RunHelper();
                return;
            }

            // If not available, return.
            if (!IsAvailable)
            {
                return;
            }

            // Fetch status, store and update.
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

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("Spotify(PlayingTime: {1}:{2:00}, CurrentTrack: {0})",
                CurrentTrack, (int) PlayingTime/60, (int) PlayingTime%60);
        }

        #region Processes

        /// <summary>
        ///     Gets whether spotify is currently running.
        /// </summary>
        public static bool IsRunning
        {
            get { return Process.GetProcessesByName("spotify").Length >= 1; }
        }

        /// <summary>
        ///     Gets whether the spotify helper is currently running.
        /// </summary>
        public static bool IsHelperRunning
        {
            get { return Process.GetProcessesByName("SpotifyWebHelper").Length >= 1; }
        }

        /// <summary>
        ///     Starts the spotify helper service.
        /// </summary>
        public static void RunHelper()
        {
            if (!IsHelperRunning)
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                              @"\Spotify\Data\SpotifyWebHelper.exe");
        }

        /// <summary>
        ///     Starts the spotify client.
        /// </summary>
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