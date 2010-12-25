using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using schaermu.utils.linq.itunes.Cache;
using System.Xml.Linq;

namespace schaermu.utils.linq.itunes.Context
{
    public static class CacheKeys
    {
        internal const string CACHEKEY_LIBRARY = "LIBRARY_DOCUMENT_CACHE";
        internal const string CACHEKEY_ARTIST_DATA = "ARTIST_CACHE";
        internal const string CACHEKEY_GENRE_DATA = "GENRE_CACHE";
        internal const string CACHEKEY_PLAYLIST_DATA = "PLAYLIST_CACHE";
        internal const string CACHEKEY_TRACK_DATA = "TRACK_CACHE";
    }
    public class ItuneContext : IDisposable
    {
        public IEnumerable<Artist> Artists
        {
            get
            {
                List<Artist> _data;
                // do lazy-loading of artist cache
                Initializers.InitializeArtistCache(out _data);
                foreach (var cArtist in _data)
                    yield return cArtist;
            }
        }

        public ItuneContext(string path)
        {
            if (!System.IO.File.Exists(path))
                throw new ApplicationException("LinqToItunes failed to initialize, path to library not valid");

            XDocument _data;
            if (!CacheProvider.Instance.TryGetItem<XDocument>(CacheKeys.CACHEKEY_LIBRARY, out _data))
            {
                _data = XDocument.Load(path);
                CacheProvider.Instance.AddItem<XDocument>(CacheKeys.CACHEKEY_LIBRARY, _data);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
