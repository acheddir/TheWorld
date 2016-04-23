using System;
using Microsoft.Extensions.Caching.Memory;

namespace TheWorld.Caching
{
    public class CacheManager : ICacheManager
    {
        private IMemoryCache _cache;

        public CacheManager(IMemoryCache cache)
        {
            _cache = cache;
        }

        #region Implementation of ICacheManager

        public T Get<T>(object key, Func<T> acquirer, double cacheTime = 1D)
        {
            T value;

            if (_cache.TryGetValue(key, out value)) return value;

            value = acquirer();
            this.Set(key, value, cacheTime);

            return value;
        }

        public void Set<T>(object key, T value, double cacheTime = 1D)
        {
            _cache.Set(key, value,
                new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(cacheTime)));
        }

        public void Remove(object key)
        {
            _cache.Remove(key);
        }

        #endregion
    }
}