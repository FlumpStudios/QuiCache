/*--------------------------------------------------------------------------------*
		QuiCacher v1.0.5 - a caching library for .NET - By Paul Marrable
            
          This libary is free to use but please leave this comment here :)

         Check out https://github.com/FlumpStudios/QuiCache for more details
*---------------------------------------------------------------------------------*/

using Microsoft.Extensions.DependencyInjection;
using System;

namespace QuiCaching
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddCachingManager(
            this IServiceCollection services,
            CachingType cachingOptions,
            Action<CachingOptions> options = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            int defaultCustomMemcacheStoreSize = 0)
        {
            if (options != null) services.Configure(options);

            if (serviceLifetime == ServiceLifetime.Scoped)
            {

                switch (cachingOptions)
                {
                    case CachingType.MemoryCache:
                        services.AddScoped<ICachingManager, MemoryCachingManager>();
                        services.AddScoped<IConfigurableCacheManager, MemoryCachingManager>();
                        services.AddSingleton<ICustomMemCache>(new CustomMemCache(defaultCustomMemcacheStoreSize));
                        break;
                    case CachingType.DistributedCache:
                        services.AddScoped<ICachingManager, DistributedCacheManager>();
                        services.AddScoped<IConfigurableCacheManager, DistributedCacheManager>();
                        break;
                    case CachingType.DoubleCache:
                        services.AddSingleton<ICustomMemCache>(new CustomMemCache(defaultCustomMemcacheStoreSize));
                        services.AddScoped<ICachingManager, DoubleCacheManager>();
                        services.AddScoped<IConfigurableCacheManager, DistributedCacheManager>();
                        break;
                }
            }

            else if (serviceLifetime == ServiceLifetime.Singleton)
            {

                switch (cachingOptions)
                {
                    case CachingType.MemoryCache:
                        services.AddSingleton<ICachingManager, MemoryCachingManager>();
                        services.AddSingleton<IConfigurableCacheManager, MemoryCachingManager>();
                        services.AddSingleton<ICustomMemCache>(new CustomMemCache(defaultCustomMemcacheStoreSize));
                        break;
                    case CachingType.DistributedCache:
                        services.AddSingleton<ICachingManager, DistributedCacheManager>();
                        services.AddSingleton<IConfigurableCacheManager, DistributedCacheManager>();
                        break;
                    case CachingType.DoubleCache:
                        services.AddSingleton<ICustomMemCache>(new CustomMemCache(defaultCustomMemcacheStoreSize));
                        services.AddSingleton<ICachingManager, DoubleCacheManager>();
                        services.AddSingleton<IConfigurableCacheManager, DistributedCacheManager>();
                        break;
                }
            }

            else if (serviceLifetime == ServiceLifetime.Transient)
            {
                switch (cachingOptions)
                {
                    case CachingType.MemoryCache:
                        services.AddTransient<ICachingManager, MemoryCachingManager>();
                        services.AddTransient<IConfigurableCacheManager, MemoryCachingManager>();
                        services.AddSingleton<ICustomMemCache>(new CustomMemCache(defaultCustomMemcacheStoreSize));
                        break;
                    case CachingType.DistributedCache:
                        services.AddTransient<ICachingManager, DistributedCacheManager>();
                        services.AddTransient<IConfigurableCacheManager, DistributedCacheManager>();
                        break;
                    case CachingType.DoubleCache:
                        services.AddSingleton<ICustomMemCache>(new CustomMemCache(defaultCustomMemcacheStoreSize));
                        services.AddTransient<ICachingManager, DoubleCacheManager>();
                        services.AddTransient<IConfigurableCacheManager, DistributedCacheManager>();
                        break;
                }
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

    public enum ServiceLifetime
    {
        Singleton,
        Scoped,
        Transient
    }

    public class CachingOptions
    {
        public TimeSpan? DefaultTimeSpan { get; set; }
        public int? DefaultMemoryEntryCacheSize { get; set; }
        public bool UseRollingIntervalAsDefault { get; set; }
    }
}
