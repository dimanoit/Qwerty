using Qwerty.BLL.Interfaces;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Qwerty.DAL.EF;
using Qwerty.DAL.Identity;

namespace Qwerty.BLL.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationContext _applicationContext;

        public AdminService(ApplicationUserManager userManager, ApplicationContext applicationContext)
        {
            _userManager = userManager;
            _applicationContext = applicationContext;
        }
        
        public async Task BlockUserAsync(string userId) =>
            await AddDeleteRole(userId, QwertyRoles.Blocked, QwertyRoles.User);
        public async Task UnblockUserAsync(string userId) =>
            await AddDeleteRole(userId, QwertyRoles.User, QwertyRoles.Blocked);

        private async Task AddDeleteRole(string userId, string roleForDelete, string roleForAdd)
        {
            var user = await _applicationContext.Users
                .FirstAsync(u => u.Id == userId);

            await _userManager.AddToRoleAsync(user, roleForAdd);
            await _userManager.RemoveFromRoleAsync(user, roleForDelete);
        }
    }
}