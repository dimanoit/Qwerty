﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Qwerty.WEB.Models
{
    public enum FriendshipRequestStatus
    {
        Rejected,
        Accepted,
        Sent,
        NotSent
    }
    public class FriendshipRequestViewModel
    {
        public FriendshipRequestStatus Status { get; set; }
        public DateTime TimeSent { get; set; }
        public string SenderUserId { get; set; }
        public string RecipientUserId { get; set; }
    }
}