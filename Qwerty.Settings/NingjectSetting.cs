using Ninject.Modules;
using Qwerty.BLL.Interfaces;
using Qwerty.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.Settings
{
    public class NingjectSetting : NinjectModule
    {
        public override void Load()
        {
            Bind<IMessageService>().To<MessageService>();
        }
    }
}
