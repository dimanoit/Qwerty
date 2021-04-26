using AutoMapper;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Qwerty.DAL.EF;

namespace Qwerty.BLL.Services
{
    public class FriendService : IFriendService, IDisposable
    {
        private IUnitOfWork _database;
        private readonly ApplicationContext _appContext;

        public FriendService(IUnitOfWork uow, ApplicationContext appContext)
        {
            _database = uow;
            _appContext = appContext;
        }

        public async Task DeleteFriend(string ThisUserId, string UserFriendId)
        {
            Friend friend = _database.FriendManager.Get(ThisUserId);
            if (friend == null) throw new ValidationException("Friend with given id does not exist.", ThisUserId);
            UserFriends ThisFriendship = _database.UserFriendsManager.Get(ThisUserId, UserFriendId);
            if (ThisFriendship == null) throw new ValidationException("ThisFriendship not exist", "UserFriends");
            _database.UserFriendsManager.Delete(ThisUserId, UserFriendId);
            await _database.SaveAsync();
        }

        public async Task AcceptFriend(string senderId, string recipientId)
        {
            var myFriend = FindFriend(senderId, recipientId);
            if (myFriend != null)
            {
                throw new ValidationException("This user already your friend", recipientId);
            }

            var iAsFriend = FindFriend(recipientId, senderId);
            if (iAsFriend != null)
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
            
            _database.UserFriendsManager.Create(friends);
            
            _database.RequestManager.Delete(recipientId, senderId);
            
            await _database.SaveAsync();
        }

        public FriendDTO FindFriend(string ThisUserId, string UserFriendId)
        {
            Friend friend = _database.UserFriendsManager.Get(UserFriendId, ThisUserId)?.Friend;
            return friend != null ? Mapper.Map<Friend, FriendDTO>(friend) : null;
        }

        public async Task<IEnumerable<UserDTO>> GetFriendsProfiles(string userId)
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
                Password = f.User.Password
            }).ToListAsync();
        }

        public void Dispose()
        {
            _database.Dispose();
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

            _database.FriendManager.Create(Mapper.Map<FriendDTO, Friend>(friendDto));
        }
    }
}