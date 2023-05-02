/*--------------------------------------------------------------------------------*
		QuiCacher v1.0.2 - a caching library for .NET - By Paul Marrable
            
          This libary is free to use but please leave this comment here :)

         Check out https://github.com/FlumpStudios/QuiCache for more details
*---------------------------------------------------------------------------------*/

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FiLogger.QuiCaching
{
    public class DistributedCacheManager : IConfigurableCacheManager
    {
        public TimeSpan DefaultTimeSpan { get; set; }
        public bool UseRollingIntervalAsDefault { get; set; }
        private readonly IDistributedCache _cache;

        private const string ENTRY_SIZE_EXCEPTION_MESSAGE = "Unable to set entry size on distributed cache";

        
        public DistributedCacheManager(
            IDistributedCache cache,
            IOptions<CachingOptions> cachingOptions)
        {
            _cache = cache;
            DefaultTimeSpan = cachingOptions.Value.DefaultTimeSpan ?? TimeSpan.FromHours(1);
            UseRollingIntervalAsDefault = cachingOptions.Value.UseRollingIntervalAsDefault;
        }

        #region GET Methods
        /// <summary>
        /// Retrieve cache asynchronously 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public async Task<T> GetCacheAsync<T,T2>(T2 key)
        {
            string value = await _cache.GetStringAsync(key.ToString());
            if (value == null) return default;

            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// Retrieve cache asynchronously 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public T GetCache<T, T2>(T2 key)
        {
            string value = _cache.GetString(key.ToString());
            if (value == null) return default;

            return JsonConvert.DeserializeObject<T>(value);
        }
        #endregion

        #region SET methods 

        /// <summary>
        /// Set cache in memory and return original object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="cacheKey"></param>
        public T SetCache<T, T2>(
            T obj,
            T2 cacheKey)
        {
            _cache.SetStringAsync(
                cacheKey.ToString(),
                SerialiseObject(obj),
                 GetCachingOption(
                    UseRollingIntervalAsDefault,
                    DefaultTimeSpan));

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
            TimeSpan? timeSpan,
            bool useRollingInterval,
            int? entrySize)
        {
            if (entrySize != null) 
                throw new ArgumentException(ENTRY_SIZE_EXCEPTION_MESSAGE);

            _cache.SetStringAsync(
                cacheKey.ToString(),
                SerialiseObject(obj),
                GetCachingOption(
                    useRollingInterval,
                    timeSpan == null ? DefaultTimeSpan : (TimeSpan)timeSpan));

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
        #endregion

        #region Private Helper Methods
        /// <summary>
        /// Set the caching options
        /// </summary>
        private static DistributedCacheEntryOptions GetCachingOption(
            bool useRollingInterval,
            TimeSpan timeSpan)
        {
            var cacheEntryOptions = new DistributedCacheEntryOptions();
            
            if (useRollingInterval)
            {
                cacheEntryOptions.SetSlidingExpiration(timeSpan);
            }
            else
            {
                var expiration = DateTime.Now + timeSpan;
                cacheEntryOptions.SetAbsoluteExpiration(expiration);
            }

            return cacheEntryOptions;
        }

        private static string SerialiseObject<T>(T obj) =>
            JsonConvert.SerializeObject(
                    obj,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
        #endregion
    }
}
