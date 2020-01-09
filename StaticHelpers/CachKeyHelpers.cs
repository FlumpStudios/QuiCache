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
