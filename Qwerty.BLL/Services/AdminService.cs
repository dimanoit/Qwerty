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
    public class AdminService : IAdminService
    {
        public IUnitOfWork Database { get; set; }

        public AdminService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task<OperationDetails> BlockUserAsync(string UserId)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(UserId);
            if (user != null)
            {
                await Database.UserManager.AddToRoleAsync(user.Id, "blocked");
                await Database.UserManager.RemoveFromRoleAsync(user.Id, "user");
                return (new OperationDetails(true, "User was blocked", "user"));
            }
            else return new OperationDetails(false, "User with this id not exists", "UserId");

        }

        public async Task<OperationDetails> UnblockUserAsync(string UserId)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(UserId);
            if (user != null)
            {
                await Database.UserManager.RemoveFromRoleAsync(user.Id, "blocked");
                await Database.UserManager.AddToRoleAsync(user.Id, "user");
                return (new OperationDetails(true, "User was Unblocked", "user"));
            }
            else return new OperationDetails(false, "User with this id not exists", "UserId");
        }
    }
}
