/*--------------------------------------------------------------------------------*
		QuiCacher v1.0.5 - a caching library for .NET - By Paul Marrable
            
          This libary is free to use but please leave this comment here :)

         Check out https://github.com/FlumpStudios/QuiCache for more details
*---------------------------------------------------------------------------------*/

using System.Threading.Tasks;

namespace QuiCaching
{
    public interface ICachingManager
    {
        Task<T> GetCacheAsync<T, T2>(T2 key);
        T GetCache<T, T2>(T2 key);
        T SetCache<T, T2>(T obj, T2 key);
        void RemoveCache<T>(params T[] keys);
    }
}
