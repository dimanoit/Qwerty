using Qwerty.DAL.EF;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Identity;
using Qwerty.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private ApplicationContext _database;
        private FriendRepository _friendManager;
        private UserRepository _qUserManager;
        private UserProfileRepository _profileManager;
        private MessageRepository _messageManager;
        private FriendshipRequestRepository _requestManager;
        private UserFriendsRepository _userFriendsManager;

        public ApplicationUserManager UserManager { get; }
        public ApplicationRoleManager RoleManager { get; }

        public UnitOfWork(ApplicationContext context, ApplicationUserManager userManager, ApplicationRoleManager applicationRoleManager)
        {
            _database = context;
            UserManager = userManager;
            RoleManager = applicationRoleManager;
        }
        public IRepositoryWithTwoKeys<UserFriends> UserFriendsManager
        {
            get
            {
                if (_userFriendsManager == null)
                    _userFriendsManager = new UserFriendsRepository(_database);
                return _userFriendsManager;
            }
        }
       
        public IRepository<Friend,string> FriendManager
        {
            get
            {
                if (_friendManager == null)
                    _friendManager = new FriendRepository(_database);
                return _friendManager;
            }
        }
        public IRepository<User,string> QUserManager
        {
            get
            {
                if (_qUserManager == null)
                    _qUserManager = new UserRepository(_database);
                return _qUserManager;
            }
        }

        public IRepository<UserProfile, string> ProfileManager
        {
            get
            {
                if (_profileManager == null)
                    _profileManager = new UserProfileRepository(_database);
                return _profileManager;
            }
        }

        public IRepository<Message, int> MessageManager
        {
            get
            {
                if (_messageManager == null)
                    _messageManager = new MessageRepository(_database);
                return _messageManager;
            }
        }

        public IRepositoryWithTwoKeys<FriendshipRequest> RequestManager
        {
            get
            {
                if (_requestManager == null)
                    _requestManager = new FriendshipRequestRepository(_database);
                return _requestManager;
            }
        }
        public async Task SaveAsync()
        {
            await _database.SaveChangesAsync();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _database.Dispose();
                }
                this.disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

