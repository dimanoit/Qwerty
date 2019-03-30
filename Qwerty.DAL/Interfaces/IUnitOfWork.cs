using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qwerty.DAL.Identity;
using Qwerty.DAL.Repositories;

namespace Qwerty.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task SaveAsync();
        ApplicationUserManager UserManager { get; }
        ApplicationRoleManager RoleManager { get; }
        FriendRepository FriendManager { get; }
        UserRepository QUserManager { get; }
        UserProfileRepository ProfileManager { get; }
        MessageRepository MessageManager { get; }
        FriendshipRequestRepository RequestManager { get; }
        UserFriendsRepository UserFriendsManager { get; }
    }
}
