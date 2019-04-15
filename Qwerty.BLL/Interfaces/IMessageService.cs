using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IMessageService
    {
        Task<OperationDetails> Send(MessageDTO messageDTO);
        MessageDTO GetMessage(int  MessageID);
        Task<OperationDetails> DeleteMessage(int MessageID);
        Task<IEnumerable<MessageDTO>> GetLastMessages(string RecipientUserId);
        Task<IEnumerable<MessageDTO>> GetAllMessagesFromDialog(string SenderId, string RecepientId);
    }
}
