using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace schaermu.utils.linq.itunes.Cache
{
    public class InMemoryCache : ICacheService
    {
        private Hashtable _storage = new HashTable();

        #region ICacheService Members

        public void AddItem<T>(string key, T data)
        {
            _storage.Add(key, data);
        }

        public void AddItem<T>(string key, T data, DateTime expiration)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(string key)
        {
            if (_storage.ContainsKey(key))
                _storage.Remove(key);
        }

        public T GetItem<T>(string key)
        {
            if (_storage.ContainsKey(key))
                return _storage[key] as T;
        }

        public bool TryGetItem<T>(string key, out T data)
        {
            try
            {
                data = (T)_storage[key];
                return true;
            }
            catch
            {
                return false;
            }
        }

        public event EventHandler<CacheServiceOutDatedEventArgs> CacheItemOutDated;

        #endregion
    }
}
