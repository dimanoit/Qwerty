using System;

namespace Qwerty.BLL.DTO
{
    public class MessageDTO
    {
            public int IdMessage { get; set; }
            public string TextMessage { get; set; }
            public DateTime DateAndTimeMessage { get; set; }
            public string IdSender { get; set; }
            public string IdRecipient { get; set; }
    }
}
