using Qwerty.BLL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qwerty.DAL.Entities;

namespace Qwerty.BLL.Interfaces
{
    public interface IFriendService
    {
        Task Delete(string firstUserId, string secondUserId);
        Task<IEnumerable<UserDTO>> GetProfiles(string userId);
        Task Accept(string senderId, string recipientId);
        Task<UserFriends> Get(string firstUserId, string secondUserId);
    }
}
