using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IServiceCreator
    {
        IUserService CreateUserService(string connection);
    }
}
