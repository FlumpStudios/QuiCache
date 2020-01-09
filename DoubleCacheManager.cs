using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FiLogger.QuiCaching
{
    public class DoubleCacheManager : ICachingManager
    {
        private readonly ICachingManager _memoryCachingManager;
        private readonly ICachingManager _distributedCacheManager;

        public DoubleCacheManager(
            ICustomMemCache memoryCache,
            IDistributedCache distributedCache,
            IOptions<CachingOptions> cachingOptions)
        {
            _memoryCachingManager = new MemoryCachingManager(memoryCache, cachingOptions);
            _distributedCacheManager = new DistributedCacheManager(distributedCache, cachingOptions);
        }

        #region GET Methods
        /// <summary>
        /// Retrieve cache asynchronously 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public async Task<T> GetCacheAsync<T, T2>(T2 key)
        {
            //Check memory cache first
            T memoryValue =  await _memoryCachingManager.GetCacheAsync<T, T2>(key);
            if (memoryValue != null) return memoryValue;

            //If memory cache null, check distributedCache
            T distributedValue = await _distributedCacheManager.GetCacheAsync<T, T2>(key);

            //If values available in distributed cache but not memory cache, update memory cache
            if (distributedValue != null)
                _memoryCachingManager.SetCache(distributedValue, key);

            return distributedValue;
        }

        /// <summary>
        /// Retrieve cache asynchronously 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public T GetCache<T, T2>(T2 key)
        {
            //Check memory cache first
            T memoryValue =  _memoryCachingManager.GetCache<T,T2>(key);
            if (memoryValue != null) return memoryValue;

            //If memory cache null, check distributedCache
            T distributedValue =  _distributedCacheManager.GetCache<T,T2>(key);

            //If values available in distributed cache but not memory cache, update memory cache
            if (distributedValue != null)
                _memoryCachingManager.SetCache(distributedValue, key);

            return distributedValue;
        }
        #endregion

        #region SET Methods
        /// <summary>
        ///  Set cache in memory and return original object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public T SetCache<T,T2>(
            T obj,
            T2 key)
        {
            _memoryCachingManager.SetCache(
                obj,
                key);

            _distributedCacheManager.SetCache(
                obj,
                key);

            return obj;
        }
        #endregion

        #region Remove Methods
        /// <summary>
        /// Remove cache on requested key
        /// </summary>
        /// <param name="keys"></param>
        public void RemoveCache<T>(params T[] keys)
        {
            _memoryCachingManager.RemoveCache(keys);
            _distributedCacheManager.RemoveCache(keys);
        }
        #endregion
    }
}
