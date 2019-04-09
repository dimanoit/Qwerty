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
                _database.FriendManager.Create(Mapper.Map<FriendDTO, Friend>(friendDto));
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

        public async Task<OperationDetails> AcceptFriend(string SenderId, string RecipientId)
        {
            FriendDTO friend = await FindFriend(SenderId, RecipientId);
            OperationDetails operationDetails = await Create(new FriendDTO { FriendId = RecipientId });
            OperationDetails operationDetails1 = await Create(new FriendDTO { FriendId = SenderId });
            _database.UserFriendsManager.Create(new UserFriends() { UserId = SenderId, FriendId = RecipientId });
            _database.RequestManager.Delete(RecipientId, SenderId);
            await _database.SaveAsync();
            return new OperationDetails(true, "Friends adedd", "friend");
        }

        public async Task<FriendDTO> FindFriend(string ThisUserId, string UserFriendId)
        {
            Friend friend = _database.UserFriendsManager.Get(UserFriendId, ThisUserId)?.Friend;
            return friend != null ? Mapper.Map<Friend, FriendDTO>(friend) : null;
        }

        public async Task<IEnumerable<UserDTO>> GetFriendsProfiles(string ThisUserId)
        {
            List<UserDTO> friendsProfilies = null;
            await Task.Run(() =>
            {
                var friendsBoof = _database.FriendManager.Get(ThisUserId).UserFriends.ToList();
                if (friendsBoof != null)
                {
                    friendsProfilies = new List<UserDTO>();
                    //foreach (var friend in friendsBoof)
                    //{
                    //    //var friendProfile = friend.UserProfile;
                    //    //friendsProfilies.Add(new UserDTO()
                    //    //{
                    //    //    Name = friendProfile.Name,
                    //    //    AboutUrl = friendProfile.AboutUrl,
                    //    //    City = friendProfile.City,
                    //    //    Country = friendProfile.Country,
                    //    //    Email = friendProfile.Email,
                    //    //    Id = friend.UserId,
                    //    //    ImageUrl = friendProfile.ImageUrl,
                    //    //    Phone = friendProfile.Phone,
                    //    //    Surname = friendProfile.Surname,
                    //    //    UserName = friend.Login,
                    //    //    Password = friendProfile.User.Password
                    //    //});
                    //}
                }
            });
            return friendsProfilies;
        }
    }
}
