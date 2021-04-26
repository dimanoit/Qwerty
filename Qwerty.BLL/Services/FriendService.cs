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

        public async Task<OperationDetails> Create(FriendDTO friendDto)
        {
            if (friendDto == null) throw new ValidationException("friendDto was null", "");
            Friend friend = _database.FriendManager.Get(friendDto.FriendId);
            if (friend != null)
                throw new ValidationException("The person already has an account as a friend.", friendDto.FriendId);
            User user = _database.QUserManager.Get(friendDto.FriendId);
            if (user == null)
                throw new ValidationException("The person already has an account as a friend.", friendDto.FriendId);
            _database.FriendManager.Create(Mapper.Map<FriendDTO, Friend>(friendDto));
            await _database.SaveAsync();
            return new OperationDetails(true, "Friend created successfully", "friend");
        }

        public async Task<OperationDetails> DeleteFriend(string ThisUserId, string UserFriendId)
        {
            Friend friend = _database.FriendManager.Get(ThisUserId);
            if (friend == null) throw new ValidationException("Friend with given id does not exist.", ThisUserId);
            UserFriends ThisFriendship = _database.UserFriendsManager.Get(ThisUserId, UserFriendId);
            if (ThisFriendship == null) throw new ValidationException("ThisFriendship not exist", "UserFriends");
            _database.UserFriendsManager.Delete(ThisUserId, UserFriendId);
            await _database.SaveAsync();
            return new OperationDetails(true, "This Friendship was deleted", "UserFriends");
        }

        public async Task<OperationDetails> AcceptFriend(string SenderId, string RecipientId)
        {
            FriendDTO friend = FindFriend(SenderId, RecipientId);
            if (friend != null) throw new ValidationException("This user already your friend", RecipientId);
            try
            {
                await Create(new FriendDTO {FriendId = RecipientId});
                await Create(new FriendDTO {FriendId = SenderId});
            }
            catch (ValidationException ex)
            {
                if (ex.Message == "The person already has an account as a friend.")
                {
                    _database.UserFriendsManager.Create(new UserFriends() {UserId = SenderId, FriendId = RecipientId});
                    _database.RequestManager.Delete(RecipientId, SenderId);
                    await _database.SaveAsync();
                    return new OperationDetails(true, "Friends adedd", "friend");
                }
                else throw new ValidationException(ex.Message, ex.Property);
            }

            _database.UserFriendsManager.Create(new UserFriends() {UserId = SenderId, FriendId = RecipientId});
            _database.RequestManager.Delete(RecipientId, SenderId);
            await _database.SaveAsync();
            return new OperationDetails(true, "Friends adedd", "friend");
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
    }
}