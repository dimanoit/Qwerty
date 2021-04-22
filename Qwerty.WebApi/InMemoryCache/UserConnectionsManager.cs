using Qwerty.WebApi.InMemoryCache.Interfaces;

namespace Qwerty.WebApi.InMemoryCache
{
    public class UserConnectionsManager : IUserConnectionsManager
    {
        private readonly ICacheManager _cacheManager;

        public UserConnectionsManager(ICacheManager cacheManager) =>
            _cacheManager = cacheManager;

        public void AddUserConnection(string userId, string connectionId) =>
            _cacheManager.Add(userId, connectionId);

        public string GetUserConnectionId(string userId) =>
            _cacheManager.Get<string>(userId);
    }
}