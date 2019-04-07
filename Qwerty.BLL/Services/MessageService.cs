using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using Qwerty.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using System.Threading.Tasks;
using Qwerty.DAL.Entities;

namespace Qwerty.BLL.Services
{
    public class MessageService : IMessageService
    {
        private IUnitOfWork _database;
        public MessageService(IUnitOfWork uow)
        {
            _database = uow;
        }
        public async Task<OperationDetails> DeleteMessage(int MessageId)
        {
            _database.MessageManager.Delete(MessageId);
            await _database.SaveAsync();
            return new OperationDetails(true, "Succses deleted", "message");
        }
        public async Task<MessageDTO> GetMessage(int MessageId)
        {
            MessageDTO message = null;
            await Task.Run(() =>
            {
                Message messageBoof = _database.MessageManager.Get(MessageId);
                message = Mapper.Map<Message, MessageDTO>(messageBoof);
            });
            return message;
        }
        public async Task<OperationDetails> Send(MessageDTO messageDTO)
        {
            Message message = _database.MessageManager.Get(messageDTO.IdMessage);
            if (message == null)
            {
                message = Mapper.Map<MessageDTO, Message>(messageDTO);
                _database.MessageManager.Create(message);
                await _database.SaveAsync();
                return new OperationDetails(true, "Message sended successfully", "message");
            }
            else return new OperationDetails(false, "This is message already exist", "message");

        }
        public async Task<IEnumerable<MessageDTO>> GetLastMessages(string RecipientUserId)
        {
            List<MessageDTO> messages = null;
            await Task.Run(() =>
            {
                User user = _database.QUserManager.Get(RecipientUserId);
                var LastMessages = (from x in user.RecivedMessages
                                    group x by x.IdSender into SenderAndYourMessages
                                    select new
                                    {
                                        LastMessageSender = SenderAndYourMessages.OrderByDescending(x => x.DateAndTimeMessage).First(),
                                        UserId = SenderAndYourMessages.Key
                                    }).Select(x => x.LastMessageSender);
                if (LastMessages != null)
                {
                    messages = new List<MessageDTO>();
                    foreach (var message in LastMessages)
                    {
                        var MessageToSender = user.SendMessages.Where(x => x.IdRecipient == message.IdSender).OrderBy(x => x.DateAndTimeMessage).LastOrDefault();
                        if (message.DateAndTimeMessage < MessageToSender?.DateAndTimeMessage)
                            messages.Add(Mapper.Map<Message, MessageDTO>(MessageToSender));
                        else
                            messages.Add(Mapper.Map<Message, MessageDTO>(message));
                    }
                }
            });
            return messages;
        }

    }
}
