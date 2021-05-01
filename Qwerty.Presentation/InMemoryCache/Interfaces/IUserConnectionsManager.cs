namespace Qwerty.WebApi.InMemoryCache.Interfaces
{
    public interface IUserConnectionsManager
    {
        void AddUserConnection(string userId, string connectionId);
        string GetUserConnectionId(string userId);
    }
}