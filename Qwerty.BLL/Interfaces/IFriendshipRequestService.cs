using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IFriendshipRequestService
    {
        Task Send(FriendshipRequestDTO friendshipRequesDTO);
        FriendshipRequestDTO GetRequest(string SenderUserId, string RecipientUserId);
        Task DeleteRequest(string SenderUserId, string RecipientUserId);
        Task<IEnumerable<FriendshipRequestDTO>> GetAllRequests(string SenderUserId);
    }
}
