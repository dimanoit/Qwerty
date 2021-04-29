using Qwerty.BLL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IFriendshipRequestService
    {
        Task Send(FriendshipRequestDTO friendshipRequestDto);
        Task<FriendshipRequestDTO> Get(string senderId, string recipientId);
        Task Delete(string senderId, string recipientId);
        Task<IEnumerable<FriendshipRequestDTO>> GetAll(string userId);
    }
}
