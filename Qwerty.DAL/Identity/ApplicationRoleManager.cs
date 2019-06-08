using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qwerty.DAL.Entities;

namespace Qwerty.DAL.Identity
{
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole> store,
            IEnumerable<IRoleValidator<IdentityRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<IdentityRole>> logger)
            : base(store, roleValidators,keyNormalizer, errors, logger)
        {
        }
    }
}
