using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpotifyAPI;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Spotify.IsRunning)
            {
                Spotify.Run();
                Thread.Sleep(1000);
            }

            Spotify spotify = new Spotify();

            Console.WriteLine(spotify);

            Console.WriteLine(spotify.CurrentTrack);

            var album = spotify.CurrentTrack.Album;

            Console.WriteLine("Current Album: {0}", album);
            Console.WriteLine("Cover art: \n{0}\n--------", string.Join("\n", album.Images.Select(i => i.URL)));
            Console.WriteLine("Tracks: \n{0}\n--------", string.Join("\n", album.Tracks));

            spotify.TrackChanged += spotify_TrackChanged;
            spotify.VolumeChanged += spotify_VolumeChanged;
            spotify.PlayStateChanged += spotify_PlayStateChanged;
            spotify.AutoUpdateInterval = 500;
            spotify.AutoUpdate = true;

            while(true) Thread.Sleep(1000);
        }

        static void spotify_PlayStateChanged(object sender, EventArgs e)
        {
            Console.WriteLine("IsPlaying changed to {0}", (sender as Spotify).IsPlaying);
        }

        static void spotify_VolumeChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Volume changed to {0}", (sender as Spotify).Volume);
        }

        static void spotify_TrackChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Track changed to {0}", (sender as Spotify).CurrentTrack);
        }
    }
}
