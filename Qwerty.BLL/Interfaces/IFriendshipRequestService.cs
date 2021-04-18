using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IFriendshipRequestService
    {
        Task<OperationDetails> Send(FriendshipRequestDTO friendshipRequesDTO);
        FriendshipRequestDTO GetRequest(string SenderUserId, string RecipientUserId);
        Task<OperationDetails> DeleteRequest(string SenderUserId, string RecipientUserId);
        Task<IEnumerable<FriendshipRequestDTO>> GetAllRequests(string SenderUserId);
    }
}
