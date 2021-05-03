using System.Collections.Generic;

namespace Qwerty.DAL.Entities
{
    public class Friend
    {
        public string FriendId { get; set; }
        public ICollection<UserFriends> UserFriends { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}