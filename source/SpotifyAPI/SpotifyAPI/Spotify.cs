using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using SpotifyAPI.Responses;

namespace SpotifyAPI
{
    public class Spotify
    {
        private readonly WebClient _webClient;

        private readonly string _csrfToken;
        private readonly string _oauthToken;

        public Spotify()
        {
            RunHelper();

            _webClient = new WebClient();

            _webClient.Headers.Add("Origin", "https://embed.spotify.com");

            _csrfToken = GetCFIDToken();
            _oauthToken = GetOAuthToken();

            Update();
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

        private static void RunHelper()
        {
            if (!IsHelperRunning)
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                              @"\Spotify\Data\SpotifyWebHelper.exe");
        }

        #endregion

        #region Tokens

        internal string GetCFIDToken()
        {
            string raw = Query("simplecsrf/token.json", false, false).Replace(@"\", "");

            var response = JsonConvert.DeserializeObject <CFIDResponse>(raw);

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
            return (string)response["t"];
        }

        #endregion

        #region Queries

        internal StatusResponse QueryStatus()
        {
            string response = Query("remote/status.json", true, true);

            if (string.IsNullOrEmpty(response)) return null;
            
            //byte[] bytes = Encoding.Default.GetBytes(response);
            //response = Encoding.UTF8.GetString(bytes);

            return JsonConvert.DeserializeObject <StatusResponse>(response);
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


        //TODO: setters

        public Track CurrentTrack { get; private set; }

        public bool IsPlaying { get; private set; }
        public bool IsShuffling { get; private set; }
        public bool IsRepeating { get; private set; }
        public bool IsPlayEnabled { get; private set; }
        public bool IsPreviousEnabled { get; private set; }
        public bool IsNextEnabled { get; private set; }
        public bool IsOnline { get; private set; }

        public double PlayingPosition { get; private set; }
        public double Volume { get; private set; }

        public int PlayingTime
        {
            get { return CurrentTrack == null ? 0 : (int)PlayingPosition; }
        }

        private void Update()
        {
            var status = QueryStatus();

            CurrentTrack = new Track(status.TrackInfo.TrackResource.Name, status.TrackInfo.TrackResource.URI,
                status.TrackInfo.Length,
                new Artist(status.TrackInfo.ArtistResource.Name, status.TrackInfo.ArtistResource.URI),
                new Album(status.TrackInfo.AlbumResource.Name, status.TrackInfo.AlbumResource.URI));

            IsPlayEnabled = status.Playing;
            IsShuffling = status.Shuffle;
            IsRepeating = status.Repeat;
            IsPlayEnabled = status.PlayEnabled;
            IsPreviousEnabled = status.PrevEnabled;
            IsNextEnabled = status.NextEnabled;
            IsOnline = status.Online;

            PlayingPosition = status.PlayingPosition;
            Volume = status.Volume;

        }

        public override string ToString()
        {
            return string.Format("Spotify(PlayingTime: {1}:{2:00}, PlayingPosition: {3}, CurrentTrack: {0})", CurrentTrack, PlayingTime / 60, PlayingTime%60, PlayingPosition);
        }
    }
}
