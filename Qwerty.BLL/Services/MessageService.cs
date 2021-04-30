using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Qwerty.DAL.EF;
using Qwerty.DAL.Entities;

namespace Qwerty.BLL.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationContext _applicationContext;

        public MessageService(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<MessageDTO> Get(int messageId)
        {
            return await _applicationContext.Messages
                .Where(m => m.IdMessage == messageId)
                .AsNoTracking()
                .ProjectTo<MessageDTO>(Mapper.Configuration)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MessageDTO>> GetLatest(string recipientId)
        {
            List<MessageDTO> resultMessages = null;
            User user = await _applicationContext.QUsers.FirstOrDefaultAsync(u => u.UserId == recipientId);
            if (user == null) return resultMessages;
            await Task.Run(() =>
            {
                var LastReceivedMessages = (from x in user.ReceivedMessages
                    group x by x.IdSender
                    into SenderAndYourMessages
                    select new
                    {
                        LastMessageSender = SenderAndYourMessages.OrderByDescending(x => x.DateAndTimeMessage).First(),
                        UserId = SenderAndYourMessages.Key
                    }).Select(x => x.LastMessageSender).ToList();
                var LastSendedMessage = (from x in user.SendMessages
                    group x by x.IdRecipient
                    into RecipientAndYourMessages
                    select new
                    {
                        LastRecipientMessage =
                            RecipientAndYourMessages.OrderByDescending(x => x.DateAndTimeMessage).First(),
                        UserId = RecipientAndYourMessages.Key
                    }).Select(x => x.LastRecipientMessage).ToList();
                resultMessages = new List<MessageDTO>();
                foreach (var SendSms in LastSendedMessage)
                {
                    var LastRecived = LastReceivedMessages.Where(x => x.IdSender == SendSms.IdRecipient)
                        .FirstOrDefault();
                    if (LastRecived != null)
                    {
                        if (SendSms.DateAndTimeMessage > LastRecived.DateAndTimeMessage)
                            resultMessages.Add(Mapper.Map<MessageDTO>(SendSms));
                        else resultMessages.Add(Mapper.Map<MessageDTO>(LastRecived));
                    }
                    else resultMessages.Add(Mapper.Map<MessageDTO>(SendSms));
                }

                foreach (var ms in LastReceivedMessages)
                {
                    if ((resultMessages.Select(x => x.IdMessage).Contains(ms.IdMessage) ||
                         resultMessages.Select(x => x.IdRecipient).Contains(ms.IdSender)) == false)
                    {
                        resultMessages.Add(Mapper.Map<MessageDTO>(ms));
                    }
                }
            });
            return resultMessages;
        }

        public async Task<IEnumerable<MessageDTO>> GetLatestNew(string recipientId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MessageDTO>> GetAllFromDialog(string senderId, string recipientId)
        {
            return await _applicationContext.Messages
                .AsNoTracking()
                .Where(m =>
                    (m.IdSender == senderId && m.IdRecipient == recipientId) ||
                    (m.IdSender == recipientId && m.IdRecipient == senderId))
                .OrderBy(m => m.DateAndTimeMessage)
                .ProjectTo<MessageDTO>(Mapper.Configuration)
                .ToListAsync();
        }


        public async Task Send(MessageDTO messageDto)
        {
            if (messageDto == null)
            {
                throw new ValidationException("This is message empty.", "message");
            }

            var message = Mapper.Map<MessageDTO, Message>(messageDto);
            _applicationContext.Messages.Add(message);

            await _applicationContext.SaveChangesAsync();
        }

        public async Task Delete(int messageId)
        {
            var message = await _applicationContext.Messages.FirstOrDefaultAsync(m => m.IdMessage == messageId);
            if (message == null)
            {
                return;
            }

            _applicationContext.Messages.Remove(message);
            await _applicationContext.SaveChangesAsync();
        }
    }
}