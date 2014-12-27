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
    /// <summary>
    ///     Represents an album's track list.
    /// </summary>
    public class TrackList : IEnumerable<Track>
    {
        #region Fields

        private readonly List<TrackListPart> _parts = new List<TrackListPart>(); //Cache of all parts

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TrackList" />.
        /// </summary>
        /// <param name="first">The first part of the list.</param>
        internal TrackList(TrackListPart first)
        {
            while (first.Offset != 0)
            {
                _parts.Insert(0, first);
                first = first.Previous;
            }
            _parts.Insert(0, first);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the number of tracks in this <see cref="TrackList" />.
        /// </summary>
        public int Count
        {
            get { return _parts.First().Total; }
        }

        /// <summary>
        ///     Gets the <see cref="Track" /> at a given index in this <see cref="TrackList" />.
        /// </summary>
        /// <param name="index">The index of which to get the <see cref="Track" />.</param>
        /// <returns>The <see cref="Track" /> at the given <paramref name="index" />.</returns>
        public Track this[int index]
        {
            get
            {
                // Bounds check
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException();

                // Looking in cached parts
                TrackListPart part =
                    _parts.FirstOrDefault(p => p.Offset <= index && p.Offset + p.Tracks.Count() > index);

                if (part != null) return part.Tracks.ElementAt(index - part.Offset);

                // Fetch parts untill we have the right part
                TrackListPart current = _parts.Last();
                while (current.Offset + current.Tracks.Count() < index)
                {
                    current = current.Next;
                    _parts.Add(current);
                }

                return current.Tracks.ElementAt(index - current.Offset);
            }
        }

        #endregion

        #region IEnumerable<Track> implementation

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Track> GetEnumerator()
        {
            for (int index = 0; index < Count; index++)
                yield return this[index];
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}