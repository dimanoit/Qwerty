using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.DAL.Entities
{
    public class Friend
    {
        public string FriendId { get; set; }
        public virtual ICollection<UserFriends> UserFriends { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public Friend()
        {
            UserFriends = new List<UserFriends>();
        }

    }
}
