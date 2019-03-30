using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.DAL.Entities
{
    public class UserFriends
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public virtual User User { get; set; }
        public virtual Friend Friend { get; set; }
    }
}
