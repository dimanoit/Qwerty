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
    }
}
