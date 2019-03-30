using AutoMapper;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.BLL.Services
{
    public class FriendshipRequestService : IFriendshipRequestService
    {
        private IUnitOfWork _database;
        public FriendshipRequestService(IUnitOfWork uow)
        {
            _database = uow;
        }
        public async Task<OperationDetails> DeleteRequest(string SenderUserId, string RecipientUserId)
        {
            _database.RequestManager.Delete(RecipientUserId, SenderUserId);
            await _database.SaveAsync();
            return new OperationDetails(true, "Succses deleted", "request");
        }

        public async Task<FriendshipRequestDTO> GetRequest(string SenderUserId, string RecipientUserId)
        {
            FriendshipRequestDTO requestDTO = null;
            await Task.Run(() =>
            {
                FriendshipRequest request = _database.RequestManager.Get(RecipientUserId, SenderUserId);
                requestDTO = Mapper.Map<FriendshipRequest, FriendshipRequestDTO>(request);
            });
            return requestDTO;
        }

        public async Task<OperationDetails> Send(FriendshipRequestDTO friendshipRequesDTO)
        {
            FriendshipRequest request = _database.RequestManager.Get(friendshipRequesDTO.RecipientUserId,friendshipRequesDTO.SenderUserId);
            if (request == null)
            {
                request = Mapper.Map<FriendshipRequestDTO, FriendshipRequest>(friendshipRequesDTO);
                _database.RequestManager.Create(request);
                await _database.SaveAsync();
                return new OperationDetails(true, "Request sended successfully", "message");
            }
            else return new OperationDetails(false, "This is request already exist", "message");
        }
    }
}
