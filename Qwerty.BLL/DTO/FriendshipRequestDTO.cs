using Qwerty.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.BLL.DTO
{
    public class FriendshipRequestDTO
    {
        public FriendshipRequestStatus Status { get; set; }
        public DateTime TimeSent { get; set; }
        public string SenderUserId { get; set; }
        public string RecipientUserId { get; set; }
    }
}
