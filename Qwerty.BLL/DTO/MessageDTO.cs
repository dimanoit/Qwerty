using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
