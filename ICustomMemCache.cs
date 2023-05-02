/*--------------------------------------------------------------------------------*
		QuiCacher v1.0.2 - a caching library for .NET - By Paul Marrable
            
          This libary is free to use but please leave this comment here :)

         Check out https://github.com/FlumpStudios/QuiCache for more details
*---------------------------------------------------------------------------------*/

using Microsoft.Extensions.Caching.Memory;

namespace FiLogger.QuiCaching
{
    public interface ICustomMemCache
    {
        MemoryCache Cache { get; set; }
    }
}