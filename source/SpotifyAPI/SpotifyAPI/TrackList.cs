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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpotifyAPI
{
    public class TrackList : IEnumerable<Track>
    {
        private readonly List<TracksListPart> _parts = new List<TracksListPart>();

        public TrackList(TracksListPart first)
        {
            while (first.Offset != 0)
            {
                _parts.Insert(0, first);
                first = first.Previous;
            }
            _parts.Insert(0, first);
        }

        public int Count
        {
            get { return _parts.First().Total; }
        }

        public Track this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException();

                TracksListPart part =
                    _parts.FirstOrDefault(p => p.Offset <= index && p.Offset + p.Tracks.Count() > index);

                if (part != null) return part.Tracks.ElementAt(index - part.Offset);


                TracksListPart current = _parts.Last();
                while (current.Offset + current.Tracks.Count() < index)
                {
                    current = current.Next;
                    _parts.Add(current);
                }

                return current.Tracks.ElementAt(index - current.Offset);
            }
        }

        public IEnumerator<Track> GetEnumerator()
        {
            for (int index = 0; index < Count; index++)
                yield return this[index];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}