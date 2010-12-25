using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace schaermu.utils.linq.itunes.Cache
{
    public class CacheServiceOutDatedEventArgs : EventArgs
    {
        public string Key { get; set; }
        public CacheServiceOutDatedEventArgs(string key)
        {
            Key = key;
        }
    }
    interface ICacheService
    {
        void AddItem<T>(string key, T data);
        void AddItem<T>(string key, T data, DateTime expiration);
        void RemoveItem(string key);
        T GetItem<T>(string key);
        bool TryGetItem<T>(string key, out T data);
        event EventHandler<CacheServiceOutDatedEventArgs> CacheItemOutDated;
    }
}
