using Microsoft.Extensions.Caching.Memory;

namespace FiLogger.QuiCaching
{
    public interface ICustomMemCache
    {
        MemoryCache Cache { get; set; }
    }
}