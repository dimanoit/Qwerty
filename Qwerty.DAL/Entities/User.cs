using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.DAL.Entities
{
    public class User
    {
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Friend> Friends { get; set; }
        public virtual ICollection<Message> SendMessages { get; set; }
        public virtual ICollection<Message> RecivedMessages { get; set; }
        public virtual ICollection<FriendshipRequest> ReciveFriendshipRequests { get; set; }
        public virtual ICollection<FriendshipRequest> SendFriendshipRequests { get; set; }
        
        public User()
        {
            Friends = new List<Friend>();
            SendMessages = new List<Message>();
            RecivedMessages = new List<Message>();
            ReciveFriendshipRequests = new List<FriendshipRequest>();
            SendFriendshipRequests = new List<FriendshipRequest>();
        }
    }
}
