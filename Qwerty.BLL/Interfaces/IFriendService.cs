using Qwerty.BLL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qwerty.DAL.Entities;

namespace Qwerty.BLL.Interfaces
{
    public interface IFriendService
    {
        Task DeleteFriend(string firstUserId, string secondUserId);
        Task<IEnumerable<UserDTO>> GetFriendsProfiles(string userId);
        Task AcceptFriend(string senderId, string recipientId);
        Task<UserFriends> GetFriendship(string firstUserId, string secondUserId);
    }
}
