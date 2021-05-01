using System;

namespace Qwerty.DAL.Entities
{
    public class Message
    {
        public int IdMessage { get; set; }
        public string TextMessage { get; set; }
        public DateTime DateAndTimeMessage { get; set; }

        public string IdSender { get; set; }
        public virtual User SenderUser { get; set; }

        public string IdRecipient { get; set; }
        public virtual User RecipientUser { get; set; }

    }
}
