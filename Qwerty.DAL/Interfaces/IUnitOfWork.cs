using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qwerty.DAL.Identity;
namespace Qwerty.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ApplicationUserManager UserManager { get; set; }
        ApplicationRoleManager RoleManager { get; set; }

    }
}
