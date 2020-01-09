using System.Threading.Tasks;

namespace FiLogger.QuiCaching
{
    public interface ICachingManager
    {
        Task<T> GetCacheAsync<T, T2>(T2 key);
        T GetCache<T, T2>(T2 key);
        T SetCache<T, T2>(T obj, T2 key);
        void RemoveCache<T>(params T[] keys);
    }
}
