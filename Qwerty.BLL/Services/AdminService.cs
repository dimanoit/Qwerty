using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using Qwerty.DAL.Interfaces;
using System;
using System.Threading.Tasks;
using Qwerty.DAL.Entities;

namespace Qwerty.BLL.Services
{
    public class AdminService : IAdminService, IDisposable
    {
        private IUnitOfWork _database;

        public AdminService(IUnitOfWork uow)
        {
            _database = uow;
        }

        public async Task BlockUserAsync(string UserId)
        {
            if (UserId == null || UserId == "") throw new ValidationException("Incorrect user id", UserId);
            User user = _database.QUserManager.Get(UserId);
            if (user == null) throw new ValidationException("User with this id not exists", UserId);
            await _database.UserManager.AddToRoleAsync(user.ApplicationUser, "blocked");
            await _database.UserManager.RemoveFromRoleAsync(user.ApplicationUser, "user");
        }

        public async Task UnblockUserAsync(string UserId)
        {
            if (UserId == null || UserId == "") throw new ValidationException("Incorrect user id", UserId);
            User user = _database.QUserManager.Get(UserId);
            if (user == null) throw new ValidationException("User with this id not exists", UserId);
            await _database.UserManager.RemoveFromRoleAsync(user.ApplicationUser, "blocked");
            await _database.UserManager.AddToRoleAsync(user.ApplicationUser, "user");
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}
