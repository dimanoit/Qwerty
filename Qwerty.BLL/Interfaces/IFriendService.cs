using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IFriendService
    {
        Task<OperationDetails> Create(FriendDTO userDto);
        Task<OperationDetails> DeleteFriend(string ThisUserId, string UserFriendId);
        FriendDTO FindFriend(string ThisUserId, string UserFriendId);
        Task<IEnumerable<UserDTO>> GetFriendsProfiles(string ThisUserId);
        Task<OperationDetails> AcceptFriend(string SenderId, string RecipientId);
    }
}
