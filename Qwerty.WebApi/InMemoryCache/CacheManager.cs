using Microsoft.Extensions.Caching.Memory;
using Qwerty.WebApi.InMemoryCache.Interfaces;

namespace Qwerty.WebApi.InMemoryCache
{
    //TODO replace by redis 
    public class CacheManager : ICacheManager
    {
        private readonly IMemoryCache _cache;

        public CacheManager(IMemoryCache cache) => _cache = cache;
        
        public void Add<T>(string key, T value) => _cache.Set(key, value);
        
        public T Get<T>(string key) => _cache.Get<T>(key);
    }
}