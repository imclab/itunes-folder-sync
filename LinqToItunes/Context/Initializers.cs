using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace schaermu.utils.linq.itunes.Context
{
   public static class Initializers
    {
       public static void InitializeArtistCache(out List<Artist> data)
       {
           if (!CacheProvider.Instance.TryGetItem<List<Artist>>(CacheKeys.CACHEKEY_ARTIST_DATA, out _data))
           {
               // load artist xml part to cache
               var _lib = CacheProvider.Instance.GetItem<XDocument>(CacheKeys.CACHEKEY_LIBRARY);
               
           }
       }
       public static void InitializeGenreCache(out List<Genre> data)
       {

       }
       public static void InitializeTrackCache(out List<Track> data)
       {

       }
       public static void InitializePlaylistCache(out List<Playlist> data)
       {

       }
    }
}
