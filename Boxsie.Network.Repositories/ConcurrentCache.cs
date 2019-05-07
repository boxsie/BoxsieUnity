using System.Collections.Concurrent;

namespace Boxsie.Network.Repositories
{
    public class ConcurrentCache<T, TY> 
    {
        private readonly ConcurrentDictionary<T, TY> _cache;
        private const int CacheRetryMax = 3;

        public ConcurrentCache()
        {
            _cache = new ConcurrentDictionary<T, TY>();
        }

        public  void Set(T key, TY val)
        {
            var cacheRetryCount = 0;
            while (!_cache.TryAdd(key, val) && cacheRetryCount < CacheRetryMax)
            {
                cacheRetryCount++;
            }
        }

        public TY Get(T key)
        {
            TY val;
            var cacheRetryCount = 0;

            while (!_cache.TryGetValue(key, out val) && cacheRetryCount < CacheRetryMax)
            {
                cacheRetryCount++;
            }

            return val;
        }
    }
}