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
    public class AdminService : IAdminService, IDisposable
    {
        private IUnitOfWork _database;

        public AdminService(IUnitOfWork uow)
        {
            _database = uow;
        }

        public async Task<OperationDetails> BlockUserAsync(string UserId)
        {
            if (UserId == null || UserId == "") throw new ValidationException("Incorrect user id", UserId);
            User user = _database.QUserManager.Get(UserId);
            if (user == null) throw new ValidationException("User with this id not exists", UserId);
            await _database.UserManager.AddToRoleAsync(user.UserId, "blocked");
            await _database.UserManager.RemoveFromRoleAsync(user.UserId, "user");
            return (new OperationDetails(true, "User was blocked", "user"));
        }

        public async Task<OperationDetails> UnblockUserAsync(string UserId)
        {
            if (UserId == null || UserId == "") throw new ValidationException("Incorrect user id", UserId);
            User user = _database.QUserManager.Get(UserId);
            if (user == null) throw new ValidationException("User with this id not exists", UserId);
            await _database.UserManager.RemoveFromRoleAsync(user.UserId, "blocked");
            await _database.UserManager.AddToRoleAsync(user.UserId, "user");
            return (new OperationDetails(true, "User was Unblocked", "user"));
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}
