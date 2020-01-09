using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace FiLogger.QuiCaching
{
    public class MemoryCachingManager : IConfigurableCacheManager
    {
        private readonly IMemoryCache _cache;        
        public TimeSpan DefaultTimeSpan { get; set; }
        public bool UseTollingIntervalAsDefault { get; set; }
        public int DefaultMemoryEntryCacheSize { get; set; }

        public MemoryCachingManager(
            ICustomMemCache cache,
            IOptions<CachingOptions> cachingOptions)
        {
            _cache = cache.Cache;
            DefaultTimeSpan = cachingOptions.Value.DefaultTimeSpan ?? TimeSpan.FromHours(1);
            UseTollingIntervalAsDefault = cachingOptions.Value.UseTollingIntervalAsDefault;
            DefaultMemoryEntryCacheSize = cachingOptions.Value.DefaultMemoryEntryCacheSize ?? 1;
        }

        #region GET methods
        /// <summary>
        /// Retrieve cache asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public async Task<T> GetCacheAsync<T, T2>(T2 key) =>
            await Task.Run(() => (T)_cache.Get(key.ToString()));


        /// <summary>
        /// Retrieve cache synchronously 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public T GetCache<T, T2>(T2 key) =>
            (T)_cache.Get(key.ToString());
        #endregion

        #region SET methods
        /// <summary>
        /// Set cache in memory and return original object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="cacheKey"></param>        
        public T SetCache<T,T2>(
            T obj,
            T2 cacheKey)
        {
            _cache.Set(
             cacheKey.ToString(),
                obj,
                GetCachingOption(
                    UseTollingIntervalAsDefault,
                    DefaultTimeSpan,
                    DefaultMemoryEntryCacheSize));

            return obj;
        }

        /// <summary>
        /// Set cache, with caching options, in memory and return original object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="cacheKey"></param>
        /// <param name="timeSpan"></param>
        /// <param name="useRollingInterval"></param>
        public T SetCache<T, T2>(
            T obj,
            T2 cacheKey,
            TimeSpan? timeSpan = null,
            bool useRollingInterval = false,
            int? entrySize = null)
        {
            _cache.Set(
                cacheKey.ToString(),
                obj,
                GetCachingOption(useRollingInterval,
                timeSpan == null ? DefaultTimeSpan : (TimeSpan)timeSpan,
                entrySize == null ? DefaultMemoryEntryCacheSize : (int)entrySize));

            return obj;
        }


        /// <summary>
        /// set cache, with caching options, via a string key and return original object.
        /// Should be used if the key is dynamic. For a static key use the Cachekeys ENUM to ensure integrity. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="timeSpan"></param>
        /// <param name="useRollingInterval"></param>
        /// <param name="entrySize"></param>
        public T SetCache<T>(
            T obj,
            string key, TimeSpan? timeSpan = null,
            bool useRollingInterval = false,
            int? entrySize = null)
        {
            _cache.Set(
                key,
                obj,
                GetCachingOption(useRollingInterval,
                timeSpan == null ? DefaultTimeSpan : (TimeSpan) timeSpan,
                entrySize == null ? DefaultMemoryEntryCacheSize : (int) entrySize));

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
            foreach (var key in keys)
                _cache.Remove(key.ToString());
        }

        /// <summary>
        /// Remove cache by string value
        /// </summary>
        /// <param name="keys"></param>
        public void RemoveCache(params string[] keys)
        {
            foreach (var key in keys)
                _cache.Remove(key);
        }

        /// <summary>
        /// Remove entries via a mix of string and enum keys
        /// </summary>
        /// <param name="keyAsEnum"></param>
        /// <param name="keyAsString"></param>
        public void RemoveCache<T>(
            T? keyAsEnum = null,
            string keyAsString = null) where T : struct, Enum
        {
            if (keyAsEnum !=null) 
                _cache.Remove(keyAsEnum.ToString());

            if (keyAsString != null) 
                _cache.Remove(keyAsString);
        }
        #endregion

        #region Private Helper Methods
        /// <summary>
        /// Set the caching options
        /// </summary>
        private MemoryCacheEntryOptions GetCachingOption(
            bool useRollingInterval,
            TimeSpan timeSpan,
            int entrySize)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(entrySize);

            if (useRollingInterval)
                cacheEntryOptions.SetSlidingExpiration(timeSpan);
            else
                cacheEntryOptions.SetAbsoluteExpiration(timeSpan);

            return cacheEntryOptions;
        }
        #endregion
    }
}

