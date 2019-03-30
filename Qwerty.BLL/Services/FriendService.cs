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
    public class FriendService : IFriendService
    {
        private IUnitOfWork _database;
        public FriendService(IUnitOfWork uow)
        {
            _database = uow;
        }
        public async Task<OperationDetails> Create(FriendDTO friendDto)
        {
            Friend friend = _database.FriendManager.Get(friendDto.FriendId);
            if (friend != null) return new OperationDetails(false, "The person already has an account as a friend.", "friend");
            else
            {
                ApplicationUser user = await _database.UserManager.FindByIdAsync(friendDto.FriendId);
                if (user == null) return new OperationDetails(false, "There is no user with this Id", "friend");
                _database.FriendManager.Create(friend);
                await _database.SaveAsync();
                return new OperationDetails(true, "Friend created successfully", "friend");
            }
        }

        public async Task<OperationDetails> DeleteFriend(string ThisUserId, string UserFriendId)
        {
            Friend friend = _database.FriendManager.Get(ThisUserId);
            if (friend == null) return new OperationDetails(false, "The person already has an account as a friend.", "friend");
            UserFriends ThisFriendship = _database.UserFriendsManager.Get(ThisUserId, UserFriendId);
            if (ThisFriendship == null) return new OperationDetails(false, "ThisFriendship not exist", "UserFriends");
            _database.UserFriendsManager.Delete(ThisUserId, UserFriendId);
            await _database.SaveAsync();
            return new OperationDetails(true, "This Friendship was deleted", "UserFriends");
        }

        public async Task<FriendDTO> FindFriend(string ThisUserId, string UserFriendId)
        {
            Friend friend = _database.UserFriendsManager.Get(ThisUserId, UserFriendId).Friend;
            return Mapper.Map<Friend, FriendDTO>(friend);
        }

        public async Task<IEnumerable<FriendDTO>> GetFriends(string ThisUserId)
        {
            List<FriendDTO> friends = null;
            await Task.Run(() =>
            {
                var friendsBoof = _database.UserFriendsManager.GetAll().Select(x => x.Friend).ToList();
                foreach (var friend in friendsBoof)
                {
                    friends.Add(Mapper.Map<Friend, FriendDTO>(friend));
                }
            });
            return friends;
        }
    }
}
