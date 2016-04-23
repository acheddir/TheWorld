using System;

namespace TheWorld.Caching
{
    public interface ICacheManager
    {
        T Get<T>(object key, Func<T> acquirer, double cacheTime = 1D);
        void Set<T>(object key, T value, double cacheTime = 1D);
        void Remove(object key);
    }
}