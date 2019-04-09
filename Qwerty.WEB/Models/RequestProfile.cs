using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Qwerty.WEB.Models
{
    public class RequestProfile
    {
        public FriendshipRequestViewModel Request { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ImageUrl { get; set; }
    }
}