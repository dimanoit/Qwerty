using Microsoft.AspNet.Identity.EntityFramework;
using Qwerty.DAL.EF;
using Qwerty.DAL.Identity;
using Qwerty.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationContext _database;

        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private FriendRepository _friendManager;
        private UserRepository _qUserManager;
        private UserProfileRepository _profileManager;
        private MessageRepository _messageManager;
        private FriendshipRequestRepository _requestManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                    _userManager = new ApplicationUserManager(new UserStore<);
                return bookRepository;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
