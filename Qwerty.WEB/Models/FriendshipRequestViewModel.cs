using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Qwerty.WEB.Models
{
    public enum FriendshipRequestStatus
    {
        NotSent,
        Rejected,
        Accepted,
        Sent,
        
    }
    public class FriendshipRequestViewModel
    {
        public FriendshipRequestStatus Status { get; set; }

        public DateTime TimeSent { get; set; }

        [Required]
        public string SenderUserId { get; set; }

        [Required]
        public string RecipientUserId { get; set; }
    }
}