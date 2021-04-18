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

namespace Qwerty.BLL.Services
{
    public class FriendService : IFriendService, IDisposable
    {
        private IUnitOfWork _database;
        public FriendService(IUnitOfWork uow)
        {
            _database = uow;
        }
        public async Task<OperationDetails> Create(FriendDTO friendDto)
        {
            if(friendDto == null ) throw new ValidationException("friendDto was null","");
            Friend friend = _database.FriendManager.Get(friendDto.FriendId);
            if (friend != null) throw new ValidationException("The person already has an account as a friend.", friendDto.FriendId);
            User user =  _database.QUserManager.Get(friendDto.FriendId);
            if (user == null) throw new ValidationException("The person already has an account as a friend.", friendDto.FriendId);
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
            FriendDTO friend =  FindFriend(SenderId, RecipientId);
            if (friend != null) throw new ValidationException("This user already your friend", RecipientId);
            try
            {
                await Create(new FriendDTO { FriendId = RecipientId });
                await Create(new FriendDTO { FriendId = SenderId });
            }catch(ValidationException ex)
            {
                if (ex.Message == "The person already has an account as a friend.")
                {
                    _database.UserFriendsManager.Create(new UserFriends() { UserId = SenderId, FriendId = RecipientId });
                    _database.RequestManager.Delete(RecipientId, SenderId);
                    await _database.SaveAsync();
                    return new OperationDetails(true, "Friends adedd", "friend");
                }
                else throw new ValidationException(ex.Message, ex.Property);
            }
            _database.UserFriendsManager.Create(new UserFriends() { UserId = SenderId, FriendId = RecipientId });
            _database.RequestManager.Delete(RecipientId, SenderId);
            await _database.SaveAsync();
            return new OperationDetails(true, "Friends adedd", "friend");
        }

        public FriendDTO FindFriend(string ThisUserId, string UserFriendId)
        {
            Friend friend = _database.UserFriendsManager.Get(UserFriendId, ThisUserId)?.Friend;
            return friend != null ? Mapper.Map<Friend, FriendDTO>(friend) : null;
        }

        public async Task<IEnumerable<UserDTO>> GetFriendsProfiles(string ThisUserId)
        {
            List<UserDTO> friendsProfilies = null;
            await Task.Run(() =>
            {
                var friendsBoof1 = _database.FriendManager.Get(ThisUserId)?.UserFriends.ToList();
                var friendsBoof2 = _database.QUserManager.Get(ThisUserId)?.UserFriends.ToList();
                friendsProfilies = new List<UserDTO>();
                if (friendsBoof1 != null)
                    foreach (var friend in friendsBoof1)
                    {
                        UserProfile friendProfile;
                        friendProfile = friend.User.UserProfile;
                        friendsProfilies.Add(new UserDTO()
                        {
                            Name = friendProfile.Name,
                            AboutUrl = friendProfile.AboutUrl,
                            City = friendProfile.City,
                            Country = friendProfile.Country,
                            Email = friendProfile.Email,
                            Id = friend.UserId,
                            ImageUrl = friendProfile.ImageUrl,
                            Phone = friendProfile.Phone,
                            Surname = friendProfile.Surname,
                            Password = friendProfile.User.Password
                        });
                    }
                if (friendsBoof2 != null)
                    foreach (var friend in friendsBoof2)
                    {
                        UserProfile friendProfile;
                        friendProfile = friend.Friend.UserProfile;
                        friendsProfilies.Add(new UserDTO()
                        {
                            Name = friendProfile.Name,
                            AboutUrl = friendProfile.AboutUrl,
                            City = friendProfile.City,
                            Country = friendProfile.Country,
                            Email = friendProfile.Email,
                            Id = friend.UserId,
                            ImageUrl = friendProfile.ImageUrl,
                            Phone = friendProfile.Phone,
                            Surname = friendProfile.Surname,
                            Password = friendProfile.User.Password
                        });
                    }
            });
            return friendsProfilies;
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}
