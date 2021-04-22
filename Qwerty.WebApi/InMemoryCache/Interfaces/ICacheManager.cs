namespace Qwerty.WebApi.InMemoryCache.Interfaces
{
    public interface ICacheManager
    {
        void Add<T>(string key, T value);
        T Get<T>(string key);
    }
}