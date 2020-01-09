using System;

namespace FiLogger.QuiCaching
{
    public interface IConfigurableCacheManager : ICachingManager
    {
        T SetCache<T, T2>(T obj, T2 key, TimeSpan? timeSpan = null, bool useRollingInterval = false, int? entrySize = null);
    }
}
