using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Qwerty.WEB.Models
{
    public class FriendViewModel
    {
        [Required]
        public string FriendId { get; set; }
    }
}