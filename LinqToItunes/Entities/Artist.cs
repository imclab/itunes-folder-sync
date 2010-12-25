using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections;
using schaermu.utils.linq.itunes.Context;

namespace schaermu.utils.linq.itunes
{
    public class Artist : IEnumerable
    {
        public string Name { get; set; }
        public int NumberOfTracks { get; set; }
        public int NumberOfAlbums { get; set; }
        public double AverageRating { get; set; }
        public IEnumerable<Genre> Genres
        {
            get
            {
                // lazy initialization of genre cache
                List<Genre> _data;
                Initializers.InitializeGenreCache(out _data);
                foreach (var item in _data)
                    yield return item;
            }
        }

        #region Implementation of IEnumerable

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
