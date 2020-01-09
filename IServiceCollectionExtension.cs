using Microsoft.Extensions.DependencyInjection;
using System;

namespace FiLogger.QuiCaching
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddCachingManager(
            this IServiceCollection services,
            CachingType cachingOptions,
            Action<CachingOptions> options = null,
            int defaultMemcacheStoreSize = 1024)
        {
            if (options != null) services.Configure(options);

            switch (cachingOptions)
            {
                case CachingType.MemoryCache:
                    services.AddTransient<ICachingManager, MemoryCachingManager>();
                    services.AddTransient<IConfigurableCacheManager, MemoryCachingManager>();
                    services.AddSingleton<ICustomMemCache>(new FiLoggerMemCache(defaultMemcacheStoreSize));
                    break;
                case CachingType.DistributedCache:
                    services.AddTransient<ICachingManager, DistributedCacheManager>();
                    services.AddTransient<IConfigurableCacheManager, DistributedCacheManager>();
                    break;
                case CachingType.DoubleCache:
                    services.AddSingleton<ICustomMemCache>(new FiLoggerMemCache(defaultMemcacheStoreSize));
                    services.AddTransient<ICachingManager, DoubleCacheManager>();
                    services.AddTransient<IConfigurableCacheManager, DistributedCacheManager>();
                    break;
            }
            return services;
        }
    }

    public enum CachingType
    {
        MemoryCache,
        DistributedCache,
        DoubleCache
    }

    public class CachingOptions
    {
        public TimeSpan? DefaultTimeSpan { get; set; }
        public int? DefaultMemoryEntryCacheSize { get; set; }
        public bool UseTollingIntervalAsDefault { get; set; }
    }
}
