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
        public virtual ICollection<User> Users { get; set; }
        public virtual UserProfile UserProfile { get; set; }

    }
}
