using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Qwerty.WebApi.InMemoryCache.Interfaces;

namespace Qwerty.WebApi.HubConfig
{
    public class NotificationHub : Hub
    {
        private readonly IUserConnectionsManager _userConnectionsManager;

        public NotificationHub(IUserConnectionsManager userConnectionsManager)
        {
            _userConnectionsManager = userConnectionsManager;
        }

        public async Task SendMessageNotification(object message, string connectionId)
            => await Clients.Client(connectionId).SendAsync(NotificationHubMethods.SendNotification, message);

        public string GetSaveConnectionId(string userId)
        {
            var connectionId = Context.ConnectionId;
             _userConnectionsManager.AddUserConnection(userId, connectionId);
             
            return connectionId;
        } 
    }
}
