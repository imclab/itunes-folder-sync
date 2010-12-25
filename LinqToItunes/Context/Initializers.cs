using System.Collections.Generic;
using System.Xml.Linq;
using schaermu.utils.linq.itunes.Cache;

namespace schaermu.utils.linq.itunes.Context
{
   public static class Initializers
    {
       public static void InitializeArtistCache(out List<Artist> data)
       {
           if (!CacheProvider.Instance.TryGetItem<List<Artist>>(CacheKeys.CACHEKEY_ARTIST_DATA, out data))
           {
               // load artist xml part to cache
               var _lib = CacheProvider.Instance.GetItem<XDocument>(CacheKeys.CACHEKEY_LIBRARY);
               
           }
       }
       public static void InitializeGenreCache(out List<Genre> data)
       {
           if (!CacheProvider.Instance.TryGetItem<List<Genre>>(CacheKeys.CACHEKEY_GENRE_DATA, out data))
           {
               // load artist xml part to cache
               var _lib = CacheProvider.Instance.GetItem<XDocument>(CacheKeys.CACHEKEY_GENRE_DATA);

           }
       }
       public static void InitializeTrackCache(out List<Track> data)
       {
           if (!CacheProvider.Instance.TryGetItem<List<Track>>(CacheKeys.CACHEKEY_TRACK_DATA, out data))
           {
               // load artist xml part to cache
               var _lib = CacheProvider.Instance.GetItem<XDocument>(CacheKeys.CACHEKEY_TRACK_DATA);

           }
       }
       public static void InitializePlaylistCache(out List<Playlist> data)
       {
           if (!CacheProvider.Instance.TryGetItem<List<Playlist>>(CacheKeys.CACHEKEY_PLAYLIST_DATA, out data))
           {
               // load artist xml part to cache
               var _lib = CacheProvider.Instance.GetItem<XDocument>(CacheKeys.CACHEKEY_PLAYLIST_DATA);

           }
       }
    }
}
