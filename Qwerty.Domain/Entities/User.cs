using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qwerty.DAL.Entities
{
    public class User
    {
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        
        // TODO delete this field we can't save user passwords
        public string Password { get; set; }
        public string Login { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        
        public virtual ICollection<UserFriends> UserFriends { get; set; }
        public virtual ICollection<Message> SendMessages { get; set; }
        public virtual ICollection<Message> ReceivedMessages { get; set; }
        public virtual ICollection<FriendshipRequest> ReceiveFriendshipRequests { get; set; }
        public virtual ICollection<FriendshipRequest> SendFriendshipRequests { get; set; }
        
        public User()
        {
            UserFriends = new List<UserFriends>();
            SendMessages = new List<Message>();
            ReceivedMessages = new List<Message>();
            ReceiveFriendshipRequests = new List<FriendshipRequest>();
            SendFriendshipRequests = new List<FriendshipRequest>();
        }
    }
}
