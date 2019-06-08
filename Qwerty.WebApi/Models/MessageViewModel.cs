using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Qwerty.WEB.Models
{
    public class MessageViewModel
    {
        public int IdMessage { get; set; }

        [MinLength(1)]
        public string TextMessage { get; set; }

        public DateTime DateAndTimeMessage { get; set; }

        [Required]
        public string IdSender { get; set; }

        [Required]
        public string IdRecipient { get; set; }
    }
}