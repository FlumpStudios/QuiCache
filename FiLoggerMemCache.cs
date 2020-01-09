using Microsoft.Extensions.Caching.Memory;

namespace FiLogger.QuiCaching
{
    public class FiLoggerMemCache : ICustomMemCache
    {
        public MemoryCache Cache { get; set; }

        public FiLoggerMemCache(int sizeLimit)
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = sizeLimit
            });
        }
    }
}
