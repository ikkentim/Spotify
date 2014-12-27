using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAPI
{
    internal static class Timestamp
    {
        public static int Generate()
        {
            return Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
        }
    }
}
