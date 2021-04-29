using AutoMapper;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Interfaces;
using Qwerty.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Qwerty.DAL.EF;

namespace Qwerty.BLL.Services
{
    public class FriendshipRequestService : IFriendshipRequestService
    {
        private readonly ApplicationContext _applicationContext;

        public FriendshipRequestService(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task Delete(string senderId, string recipientId)
        {
            var request = await _applicationContext.Requests.FirstOrDefaultAsync(r =>
                r.SenderUserId == senderId && r.RecipientUserId == recipientId);

            if (request == null)
            {
                return;
            }

            _applicationContext.Requests.Remove(request);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task<FriendshipRequestDTO> Get(string senderId, string recipientId)
        {
            var request = await _applicationContext.Requests.FirstOrDefaultAsync(r =>
                r.SenderUserId == senderId && r.RecipientUserId == recipientId);

            return request == null ? null : Mapper.Map<FriendshipRequest, FriendshipRequestDTO>(request);
        }
        
        public async Task<IEnumerable<FriendshipRequestDTO>> GetAll(string userId)
        {
            return await _applicationContext.Requests
                .Where(r => r.SenderUserId == userId || r.RecipientUserId == userId)
                .ProjectTo<FriendshipRequestDTO>(Mapper.Configuration)
                .ToListAsync();
        }

        public async Task Send(FriendshipRequestDTO friendshipRequestDto)
        {
            var request = await Get(friendshipRequestDto.RecipientUserId, friendshipRequestDto.SenderUserId);
            if (request != null)
            {
                return;
            }

            var newRequest = Mapper.Map<FriendshipRequestDTO, FriendshipRequest>(friendshipRequestDto);
            newRequest.Status = FriendshipRequestStatus.Sent;
            newRequest.TimeSent = DateTime.Now;

            _applicationContext.Requests.Add(newRequest);
            await _applicationContext.SaveChangesAsync();
        }

      
    }
}