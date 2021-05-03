using System.Collections.Generic;

namespace Qwerty.DAL.Entities
{
    public class User
    {
        public string UserId { get; set; }
        public string Login { get; set; }

        public UserProfile UserProfile { get;  }
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<UserFriends> UserFriends { get; }
        public ICollection<Message> SendMessages { get; }
        public ICollection<Message> ReceivedMessages { get; }
        public ICollection<FriendshipRequest> ReceiveFriendshipRequests { get; }
        public ICollection<FriendshipRequest> SendFriendshipRequests { get; }
    }
}