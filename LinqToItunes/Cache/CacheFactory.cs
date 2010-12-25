using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace schaermu.utils.linq.itunes.Cache
{
    public static class CacheFactory
    {
        internal static ICacheService Create(string cacheService)
        {
            // locate type
            var t = Type.GetType(cacheService);
            if (t == null) return default(ICacheService);

            // create new instance
            var retCache = Activator.CreateInstance(t) as ICacheService;
            return retCache ?? default(ICacheService);
        }
    }
}
