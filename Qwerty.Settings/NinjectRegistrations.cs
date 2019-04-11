using Ninject.Modules;
using Qwerty.BLL.Interfaces;
using Qwerty.BLL.Services;
using Qwerty.DAL.Interfaces;
using Qwerty.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.Settings
{
    public class NinjectRegistrations : NinjectModule
    {
        public override void Load()
        {
            Bind<IMessageService>().To<MessageService>().InSingletonScope();
            Bind<IFriendService>().To<FriendService>().InSingletonScope();
            Bind<IFriendshipRequestService>().To<FriendshipRequestService>().InSingletonScope();
            Bind<IAdminService>().To<AdminService>().InSingletonScope();
            Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("DefaultConnection");
        }
    }
}