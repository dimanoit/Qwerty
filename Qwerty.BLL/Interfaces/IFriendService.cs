using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IFriendService
    {
        Task Create(FriendDTO userDto);
        Task DeleteFriend(string ThisUserId, string UserFriendId);
        FriendDTO FindFriend(string ThisUserId, string UserFriendId);
        Task<IEnumerable<UserDTO>> GetFriendsProfiles(string ThisUserId);
        Task AcceptFriend(string senderId, string recipientId);
    }
}
