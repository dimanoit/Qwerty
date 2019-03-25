using Microsoft.AspNet.Identity.EntityFramework;
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

        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private FriendRepository _friendManager;
        private UserRepository _qUserManager;
        private UserProfileRepository _profileManager;
        private MessageRepository _messageManager;
        private FriendshipRequestRepository _requestManager;

        public UnitOfWork(string connectionString)
        {
            _database = new ApplicationContext(connectionString);
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                    _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_database));
                return _userManager;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                if (_roleManager == null)
                    _roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(_database));
                return _roleManager;
            }
        }

        public FriendRepository FriendManager
        {
            get
            {
                if (_friendManager == null)
                    _friendManager = new FriendRepository(_database);
                return _friendManager;
            }
        }
        public UserRepository QUserManager
        {
            get
            {
                if (_qUserManager == null)
                    _qUserManager = new UserRepository(_database);
                return _qUserManager;
            }
        }

        public UserProfileRepository ProfileManager
        {
            get
            {
                if (_profileManager == null)
                    _profileManager = new UserProfileRepository(_database);
                return _profileManager;
            }
        }

        public MessageRepository MessageManager
        {
            get
            {
                if (_messageManager == null)
                    _messageManager = new MessageRepository(_database);
                return _messageManager;
            }
        }

        public FriendshipRequestRepository RequestManager
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

