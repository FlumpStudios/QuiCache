/*--------------------------------------------------------------------------------*
		QuiCacher v1.0.5 - a caching library for .NET - By Paul Marrable
            
          This libary is free to use but please leave this comment here :)

         Check out https://github.com/FlumpStudios/QuiCache for more details
*---------------------------------------------------------------------------------*/

using System;
using System.Threading.Tasks;

namespace QuiCaching
{
    public class NoCacheManager : IConfigurableCacheManager
    {

        #region GET Methods
        /// <summary>
        /// Retrieve cache asynchronously, as this is the  no cache manager, default is returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public async Task<T> GetCacheAsync<T, T2>(T2 key) =>
            await Task.Run(() => (T)default);


        /// <summary>
        /// Retrieve cache synchronously, as this is the no cache manager, default is returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public T GetCache<T, T2>(T2 key) => default;

        #endregion

        #region SET Methods
        /// <summary>
        ///  Set cache in memory and return original object, as this is no cache manager, return the same object that is supplied
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public T SetCache<T, T2>(
            T obj,
            T2 key) => obj;

        /// <summary>
        /// Set cache, with caching options, in memory and return original object. as this is no cache manager, return the same object that is supplied
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
            int? entrySize = null) => obj;


        #endregion

        #region Remove Methods
        /// <summary>
        /// Remove cache on requested key
        /// </summary>
        /// <param name="keys"></param>
        public void RemoveCache<T>(params T[] keys)
        {
            //As there is no cache enabled...don't do anything.
        }
        #endregion
    }
}
