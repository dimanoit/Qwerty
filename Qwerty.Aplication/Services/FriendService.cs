using AutoMapper;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using Qwerty.DAL.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Qwerty.DAL.EF;

namespace Qwerty.BLL.Services
{
    public class FriendService : IFriendService
    {
        private readonly ApplicationContext _appContext;

        public FriendService(ApplicationContext appContext)
        {
            _appContext = appContext;
        }

        public async Task Delete(string firstUserId, string secondUserId)
        {
            var friendship = await Get(firstUserId, secondUserId);
            
            if (friendship != null)
            {
                _appContext.UserFriends.Remove(friendship);
            }

            await _appContext.SaveChangesAsync();
        }

        public async Task Accept(string senderId, string recipientId)
        {
            var friendShip = await Get(senderId, recipientId);
            if (friendShip != null)
            {
                throw new ValidationException("This user already your friend", recipientId);
            }

            await CreateFriendAccountIfNotExist(new FriendDTO {FriendId = recipientId});

            await CreateFriendAccountIfNotExist(new FriendDTO {FriendId = senderId});

            var friends = new UserFriends
            {
                UserId = senderId,
                FriendId = recipientId
            };

            _appContext.UserFriends.Add(friends);

            var requestForRemove = await _appContext.Requests
                .FirstAsync(r => r.SenderUserId == senderId && r.RecipientUserId == recipientId);

            _appContext.Requests.Remove(requestForRemove);

            await _appContext.SaveChangesAsync();
        }

        public async Task<UserFriends> Get(string firstUserId, string secondUserId)
        {
            return await _appContext.UserFriends.FirstOrDefaultAsync(uf =>
                (uf.FriendId == firstUserId && uf.UserId == secondUserId) ||
                (uf.FriendId == secondUserId && uf.UserId == firstUserId)
            );
        }

        public async Task<IEnumerable<UserDTO>> GetProfiles(string userId)
        {
            var friends = from p in _appContext.Profiles
                join uf in _appContext.UserFriends
                    on p.UserId equals uf.FriendId == userId ? uf.UserId : uf.UserId == userId ? uf.FriendId : null
                select p;

            // TODO create autoMapper mapping for this entities
            return await friends.Select(f => new UserDTO
            {
                Name = f.Name,
                AboutUrl = f.AboutUrl,
                City = f.City,
                Country = f.Country,
                Email = f.Email,
                Id = f.UserId,
                ImageUrl = f.ImageUrl,
                Phone = f.Phone,
                Surname = f.Surname,
            }).ToListAsync();
        }

        private async Task CreateFriendAccountIfNotExist(FriendDTO friendDto)
        {
            if (friendDto == null)
            {
                throw new ValidationException("friendDto was null", "");
            }

            var friend = await _appContext.Friends.FirstOrDefaultAsync(f => f.FriendId == friendDto.FriendId);
            if (friend != null)
            {
                return;
            }

            var newFriend = Mapper.Map<FriendDTO, Friend>(friendDto);
            _appContext.Friends.Add(newFriend);
        }
    }
}