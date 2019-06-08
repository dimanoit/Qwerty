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
    public class FriendshipRequestService : IFriendshipRequestService, IDisposable
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

        public FriendshipRequestDTO GetRequest(string SenderUserId, string RecipientUserId)
        {
            FriendshipRequestDTO requestDTO = null;
            FriendshipRequest request = _database.RequestManager.Get(RecipientUserId, SenderUserId);
            if(request != null)
            requestDTO = Mapper.Map<FriendshipRequest, FriendshipRequestDTO>(request);
            return requestDTO;
        }
        
        public async Task<OperationDetails> Send(FriendshipRequestDTO friendshipRequesDTO)
        {

            FriendshipRequest request = _database.RequestManager.Get(friendshipRequesDTO.RecipientUserId, friendshipRequesDTO.SenderUserId);
            if (request != null) throw new ValidationException("This is request already exist", "message");
            request = Mapper.Map<FriendshipRequestDTO, FriendshipRequest>(friendshipRequesDTO);
            request.Status = FriendshipRequestStatus.Sent;
            request.TimeSent = DateTime.Now;
            _database.RequestManager.Create(request);
            await _database.SaveAsync();
            return new OperationDetails(true, "Request sended successfully", "message");
        }

        public async Task<IEnumerable<FriendshipRequestDTO>> GetAllRequests(string SenderUserId)
        {
            ICollection<FriendshipRequestDTO> requestDTO = null;
            await Task.Run(() =>
            {
                User user = _database.QUserManager.Get(SenderUserId);
                if (user != null)
                {
                    var ReciveRequests = user.ReceiveFriendshipRequests;
                    var SendRequest = user.SendFriendshipRequests;
                    if (ReciveRequests != null || SendRequest != null) requestDTO = new List<FriendshipRequestDTO>();
                    if (ReciveRequests != null)
                    {
                        foreach (var el in ReciveRequests)
                        {
                            requestDTO.Add(Mapper.Map<FriendshipRequest, FriendshipRequestDTO>(el));
                        }
                    }
                    if (SendRequest != null)
                    {
                        foreach (var el in SendRequest)
                        {
                            requestDTO.Add(Mapper.Map<FriendshipRequest, FriendshipRequestDTO>(el));
                        }
                    }
                }
            });
            return requestDTO;
        }
        public void Dispose()
        {
            _database.Dispose();
        }

    }
}
