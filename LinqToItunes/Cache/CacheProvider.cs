using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace schaermu.utils.linq.itunes.Cache
{
    public static class CacheProvider
    {
        private static readonly object _lock = new object();
        // TODO: make selection of cacheservice dynamic/configureable
        private static readonly ICacheService _instance = CacheFactory.Create("InMemoryCache");

        public static ICacheService Instance
        {
            get
            {
                if (_instance != null)
                {
                    lock (_lock)
                    {
                        if (_instance != null)
                            return _instance;
                    }
                }
                return null;
            }
        }

        CacheProvider() { }
    }
}
