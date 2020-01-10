/*--------------------------------------------------------------------------------*
		   QuiCacher - a caching library for .NET core - By Paul Marrable
            
          This libary is free to use but please leave this comment here :)

         Check out https://github.com/FlumpStudios/QuiCache for more details
*---------------------------------------------------------------------------------*/

using System;

namespace FiLogger.QuiCaching
{
    public interface IConfigurableCacheManager : ICachingManager
    {
        T SetCache<T, T2>(T obj, T2 key, TimeSpan? timeSpan = null, bool useRollingInterval = false, int? entrySize = null);
    }
}
