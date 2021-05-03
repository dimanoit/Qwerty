using System;

namespace Qwerty.DAL.Entities
{
    public class Message
    {
        public int IdMessage { get; set; }
        public string TextMessage { get; set; }
        public DateTime DateAndTimeMessage { get; set; }

        public string IdSender { get; set; }
        public User SenderUser { get; set; }

        public string IdRecipient { get; set; }
        public User RecipientUser { get; set; }
    }
}