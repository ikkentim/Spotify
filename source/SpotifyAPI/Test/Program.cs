using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Spotify spotify = new Spotify();

            Console.WriteLine(spotify);

            Console.WriteLine(spotify.CurrentTrack);

            Console.WriteLine(spotify.CurrentTrack.Album);

            Console.WriteLine(spotify.CurrentTrack.AlbumImages.Length);

            Console.ReadLine();
        }
    }
}
