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
        public MessageDTO GetMessage(int MessageId)
        {
            MessageDTO message = null;
            Message messageBoof = _database.MessageManager.Get(MessageId);
            if (messageBoof != null) message = Mapper.Map<Message, MessageDTO>(messageBoof);
            return message;
        }
        public async Task<OperationDetails> Send(MessageDTO messageDTO)
        {
            if(messageDTO == null) throw new ValidationException("This is message empty.", "message");
            Message message = _database.MessageManager.Get(messageDTO.IdMessage);
            if (message != null) throw new ValidationException("This is message already exist", "message");
            message = Mapper.Map<MessageDTO, Message>(messageDTO);
            _database.MessageManager.Create(message);
            await _database.SaveAsync();
            return new OperationDetails(true, "Message sended successfully", "message");

        }
        public async Task<IEnumerable<MessageDTO>> GetLastMessages(string RecipientUserId)
        {
            List<MessageDTO> resultMessages = null;
            User user = _database.QUserManager.Get(RecipientUserId);
            if (user == null) return resultMessages;
            await Task.Run(() =>
            {
                var LastReceivedMessages = (from x in user.RecivedMessages
                                            group x by x.IdSender into SenderAndYourMessages
                                            select new
                                            {
                                                LastMessageSender = SenderAndYourMessages.OrderByDescending(x => x.DateAndTimeMessage).First(),
                                                UserId = SenderAndYourMessages.Key
                                            }).Select(x => x.LastMessageSender).ToList();
                var LastSendedMessage = (from x in user.SendMessages
                                         group x by x.IdRecipient into RecipientAndYourMessages
                                         select new
                                         {
                                             LastRecipientMessage = RecipientAndYourMessages.OrderByDescending(x => x.DateAndTimeMessage).First(),
                                             UserId = RecipientAndYourMessages.Key
                                         }).Select(x => x.LastRecipientMessage).ToList();
                resultMessages = new List<MessageDTO>();
                foreach (var SendSms in LastSendedMessage)
                {
                    var LastRecived = LastReceivedMessages.Where(x => x.IdSender == SendSms.IdRecipient).FirstOrDefault();
                    if (LastRecived != null)
                    {
                        if (SendSms.DateAndTimeMessage > LastRecived.DateAndTimeMessage) resultMessages.Add(Mapper.Map<MessageDTO>(SendSms));
                        else resultMessages.Add(Mapper.Map<MessageDTO>(LastRecived));
                    }
                    else resultMessages.Add(Mapper.Map<MessageDTO>(SendSms));
                }
                foreach (var ms in LastReceivedMessages)
                {
                    if ((resultMessages.Select(x => x.IdMessage).Contains(ms.IdMessage) || resultMessages.Select(x => x.IdRecipient).Contains(ms.IdSender)) == false)
                    {
                        resultMessages.Add(Mapper.Map<MessageDTO>(ms));
                    }
                }
            });
            return resultMessages;
        }
        public async Task<IEnumerable<MessageDTO>> GetAllMessagesFromDialog(string SenderId, string RecepientId)
        {
            List<MessageDTO> Messages = null;
            User Sender = _database.QUserManager.Get(SenderId);
            User Recipient = _database.QUserManager.Get(SenderId);
            if (Sender == null || Recipient == null) return Messages;
            else Messages = new List<MessageDTO>();
            await Task.Run(() =>
            {
                foreach (var message in Sender.SendMessages.Where(x => x.IdRecipient == RecepientId))
                {
                    Messages.Add(Mapper.Map<Message, MessageDTO>(message));
                }
                foreach (var message in Sender.RecivedMessages.Where(x => x.IdSender == RecepientId))
                {
                    Messages.Add(Mapper.Map<Message, MessageDTO>(message));
                }
            });
            return Messages.OrderBy(x => x.DateAndTimeMessage);
        }

    }
}
