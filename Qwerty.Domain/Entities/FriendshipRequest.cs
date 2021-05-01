using System;

namespace Qwerty.DAL.Entities
{
    public class FriendshipRequest
    {
        public FriendshipRequestStatus Status { get; set; }
        public DateTime TimeSent { get; set; }


        public string SenderUserId { get; set; } 
        public virtual User SenderUser { get; set; }

        public string RecipientUserId { get; set; }
        public virtual User RecipientUser { get; set; }
    }
}
