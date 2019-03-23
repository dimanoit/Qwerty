using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Qwerty.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.DAL.Identity
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole> store) : base(store) { }
    }
}
