<h1>QicCache</h1>
<p>Caching library to simplify caching in .NET Core 2.2+</p>
<h2>Info</h2>
<p>QicCache is a small library to help keep memory and distributed cache in .NET as simple and clean as possible. QicCache abstracts out the caching and allows for easy setup, configuration and switching between caching types.</p>
<h2>Setup</h2>
<h4>Add memory cache -<small> only needed if memory cache or double cache is to be used</small></h4>
<ul>
<li>Install Microsoft caching by running the following in the package manager console: <code>Install-Package System.Runtime.Caching -Version 4.7.0</code></li>
<li>Add the memory cache service in the <code>ConfigureServices</code> method in your startup class: <code>services.AddMemoryCache();</code></li>
</ul>
<p>That should be the memory cache all setup, nice and simple. If you need more in-depth instructions or are still having problems, check out the full memory cache docs <a href="https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-2.2" target="_blank" rel="noopener"> here </a></p>
<h4>Add distributed cache -<small> only needed if distributed cache or double cache is to be used</small></h4>
<p>There are 3 types of distributed cache that can be used with QicCache, distributed memory cache, SQL cache or Redis Cache. I would recommend using Redis cache, so I will only go through the setup process for Redis cache. If you would like to use SQL or distributed memory, please follow the official .NET distributed cache documentation <a href="https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-2.2" target="_blank" rel="noopener"> here </a></p>
<p>Here are the steps to get Redis cache up and running</p>
<ul>
<li>The first step is to install Redis. You can find the installation guides here -&nbsp;<a href="https://redislabs.com/blog/redis-on-windows-10/" target="_blank" rel="noopener">Windows 10</a> / <a href="https://redis.io/topics/quickstart" target="_blank" rel="noopener"> Linux</a> / <a href="https://medium.com/@petehouston/install-and-config-redis-on-mac-os-x-via-homebrew-eb8df9a4f298" target="_blank" rel="noopener">Mac</a></li>
<li>Start the Redis service. This should have been covered in the above guides, but basically you need to restart the service with <code>sudo service redis-server restart</code> then run <code>redis-cli</code></li>
<li>That's your Redis store all set up, now you need to install the Redis caching package in your project. Run <code>Install-Package Microsoft.Extensions.Caching.StackExchangeRedis -Version 3.1.0</code> in the package manager console.</li>
<li>Now register the Redis Service in the <code>ConfigureServices&nbsp;</code>method of your service class
<pre>                services.AddDistributedRedisCache(option =&gt;
            {
                option.Configuration = "127.0.0.1";
                option.InstanceName = "master";
            });
  </pre>
</li>
</ul>
<p>OK, that should be it for setting up your Redis cache. If you have any problems, check out the official Redis docs <a href="https://redis.io/documentation" target="_blank" rel="noopener">here</a>, or the official .NET distributed cache docs <a href=" &lt;a href=" target="_blank" rel="noopener">Here</a></p>
<h4>Setup QuiCache</h4>
<ul>
<li>Add the QuiCache service in the <code>ConfigureServices</code> method in your startup class: <code>services.AddCachingManager(CachingType.MemoryCache)</code></li>
<li>Select which type of caching you would like. CachingType is an Enum that gives you three options, MemoryCache, Distributed Cache or Double Cache*.</li>
<li>Configure your caching. The <code>AddCachingManager</code> method takes 2 optional arguments to configure your store, the first optional argument takes an action delegate which allows you to set up interval type, default memory cache size, and default Timespan. The second default argument allows you to set a default memory cache size limit. Here's an example
<pre>          services.AddCachingManager(cachingType,
                options =&gt; 
                {
                    options.UseTollingIntervalAsDefault = true;
                    options.DefaultMemoryEntryCacheSize = 1;
                    options.DefaultTimeSpan = TimeSpan.FromMinutes(30);
                },
                 2040);
  </pre>
</li>
</ul>
<p>OK, that should be all the setup out of the way, you should now be ready to add caching to your project.</p>
<p><small><strong>*</strong>Double cache will check the memory cache first, if there's nothing there it will check the distributed cache if there's data in the distributed cache but not the memory cache, the memory cache is updated with the distributed cache data. This maximises caching speed but comes at the cost of having fewer options when caching data. </small></p>
<h2>Application</h2>
<h4>Basic usage</h4>
<p>Now you're all setup, this bit should be easy. Basically, you need to check if there's any data in the cache based on your cache key, if there isn't, then you need to get the data from you data source and update the cache with the new data. So how do we do this? Here's a basic example.</p>
<pre>	CacheKeysEnum yourCacheKey = CacheKeys.Foo;

	private readonly ICachingManager _cachingController;
 	
	public YourConstructor(ICachingManager cachingController)
	{
		_cachingController = cachingController;
	}
	
    public Object YourMethod()
	{
		//Check the cache store first
		var cachedData =  await _cachingController.GetCacheAsync&lt;YourObjectType, Enum&gt;(yourCacheKey);

		//If there's data in the cache store return data
		if (cachedData != null) return cachedData;

		//If not data in cache, get the data from the data source
		var dataFromDataSource = await _repository.YourData();

		//Update the cache with the new data
		_cachingController.SetCache(dataFromDataSource, yourCacheKey);

		//Return the data
		return dataFromDataSource;
    }
</pre>
<p>As the SetCache methods returns the object that is passed to it, you can be one of the cool kids and use the null-coalescing operator to get and set the cache. So the above cached method becomes...</p>
<pre>	
	public Object YourMethod()
	{
		CacheKeysEnum yourCacheKey = CacheKeys.Foo;

		return 
			await _cachingController.GetCacheAsync&lt;YourObjectType, Enum&gt;(CacheKeysEnum) 
			?? _cachingController.SetCache(await _repository.YourData(),CacheKeysEnum);
	}
</pre>

<p>&nbsp;</p>
<h4>The GetCacheAsync method</h4>
<p>As you may have guessed, this is the method we use to retrieve data from the cache-store. This function takes two generic type parameters, the object type, and the cache key type.</p>
<p>You can use any stringable object as a cache key, but I would recommend using enums as this reduces the chance of typos and clashing keys. If there is no data in the store, the methods will return null.</p>
<h4>The SetCacheAsync method</h4>
<p>This is where we set our data int he cache-store. This function has 2 mandatory parameters, the object you are caching and the cache key.</p>
<p>This method returns the request cache object</p>
<h4>The RemoveCache Method</h4>
<p>This method removes cache from the store based on the cache key. This method takes a parameter list of keys and removes any data in the store that matches the passed keys.</p>
<pre> _cachingController.RemoveCache(yourCacheKey.foo, yourCacheKey.bar. "String Key");
</pre>
<h4>IConfigurableCacheManager interface</h4>
<p>If you require more control over a cache entry, you can inject the caching manager via the IConfigurableCacheManager interface instead of the ICachingManager interface.</p>
<p>This exposes an overflow method for SetCache which allows you to set the cache with some additional options.</p>
<p>Here is the signature of the overflow method.</p>
<pre>   T SetCache&lt;T, T2&gt;(T obj, T2 key, TimeSpan? timeSpan = null, bool useRollingInterval = false, int? entrySize = null);
   </pre>
<p>You already know the 2 first parameters, but what are the others?</p>
<p><strong>TimeSpan - </strong>This allows you to specify how long you would like the entry to live in the cache.</p>
<p><strong>useRollingInterval - </strong> This allows you to use a rolling interval for the entry</p>
<p><strong>entrySize -</strong> This is only available to memory cache, this allows you to specify the size of the cache entry in the memory cache.</p>


<h2>Summary</h2>
<p>If you couldn't be bothered to read all that, here's a quick run down on how to get started with QuiCache</p>
<ul>
<li><a href="https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-2.2" target="_blank" rel="noopener"> Add memory cache to project </a></li>
<li>Install Redis. installation guides here -&nbsp;<a href="https://redislabs.com/blog/redis-on-windows-10/" target="_blank" rel="noopener">Windows 10</a> / <a href="https://redis.io/topics/quickstart" target="_blank" rel="noopener"> Linux</a> / <a href="https://medium.com/@petehouston/install-and-config-redis-on-mac-os-x-via-homebrew-eb8df9a4f298" target="_blank" rel="noopener">Mac</a></li>
<li><a href="https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-2.2" target="_blank" rel="noopener">Add distributed cache to .NET</a></li>
<li>Add QuiCache cachingManager to startup class like this - <code>services.AddCachingManager(cachingType);</code></li>
<li>Inject into class where you want to cache data. Like this...
<pre>	private readonly ICachingManager _cachingController;
 	
	public YourConstructor(ICachingManager cachingController)
	{
		_cachingController = cachingController;
	}
</pre>
</li>
<li>Cache your data, like this...
<pre>    
    	await _cachingController.GetCacheAsync&lt;YourObjectType, Enum&gt;(CacheKeysEnum) 
	?? _cachingController.SetCache(await _repository.YourData(),CacheKeysEnum);</pre>
</li>
</ul>
<p>So that's pretty much it, hopefully, you can find this library useful.</p>
