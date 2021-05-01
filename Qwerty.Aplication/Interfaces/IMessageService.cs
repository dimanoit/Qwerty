using Qwerty.BLL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IMessageService
    {
        Task Send(MessageDTO messageDto);
        Task<MessageDTO> Get(int messageId);
        Task Delete(int messageId);
        Task<IEnumerable<MessageDTO>> GetLatest(string recipientId);
        Task<IEnumerable<MessageDTO>> GetAllFromDialog(string senderId, string recipientId);
    }
}
