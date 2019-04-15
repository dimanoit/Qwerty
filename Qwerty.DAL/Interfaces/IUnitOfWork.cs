using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Identity;
using Qwerty.DAL.Repositories;

namespace Qwerty.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task SaveAsync();
        ApplicationUserManager UserManager { get; }
        ApplicationRoleManager RoleManager { get; }
        IRepository<Friend, string> FriendManager { get; }
        IRepository<User, string>  QUserManager { get; }
        IRepository<UserProfile, string> ProfileManager { get; }
        IRepository<Message, int> MessageManager { get; }
        IRepositoryWithTwoKeys<FriendshipRequest> RequestManager { get; }
        IRepositoryWithTwoKeys<UserFriends> UserFriendsManager { get; }
    }
}
