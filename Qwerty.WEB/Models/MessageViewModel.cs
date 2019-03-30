using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Qwerty.WEB.Models
{
    public class MessageViewModel
    {
        public int IdMessage { get; set; }
        public string TextMessage { get; set; }
        public DateTime DateAndTimeMessage { get; set; }
        public string IdSender { get; set; }
        public string IdRecipient { get; set; }
    }
}