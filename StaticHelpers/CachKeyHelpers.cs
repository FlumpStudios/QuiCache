/*--------------------------------------------------------------------------------*
		   QuiCacher - a caching library for .NET core - By Paul Marrable
            
          This libary is free to use but please leave this comment here :)

         Check out https://github.com/FlumpStudios/QuiCache for more details
*---------------------------------------------------------------------------------*/

using System.Collections.Generic;

namespace FiLogger.QuiCaching.StaticHelpers
{
    public static class CachKeyHelpers
    {
        public static string GenerateUniqueCacheKey<T>(
           object cacheKey,
           params T[] uids)
        {
            List<string> x = new List<string>() { cacheKey.ToString() };
            foreach (var uid in uids)
            {
                x.Add(uid.ToString());
            }

            return string.Join("_", x);
        }
    }
}
