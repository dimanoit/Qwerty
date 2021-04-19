using System.Linq;
using Microsoft.AspNetCore.Identity;
using Qwerty.DAL.Identity;

namespace Qwerty.DAL.EF
{
    public static class DbInitializer
    {
        public static void SeedData(ApplicationContext dbContext)
        {
            AddRolesIfNotExist(dbContext, QwertyRoles.Admin, QwertyRoles.Blocked, QwertyRoles.User);
        }

        private static void AddRolesIfNotExist(ApplicationContext dbContext, params string[] roles)
        {
            foreach (var role in roles)
            {
                var requestedRole = dbContext.Roles.FirstOrDefault(c => c.Name == role);

                if (requestedRole == null)
                {
                    requestedRole = new IdentityRole
                    {
                        NormalizedName = role.ToUpper(),
                        Name = role
                    };

                    dbContext.Roles.Add(requestedRole);
                    dbContext.SaveChanges();
                }
            }
        }
    }
}