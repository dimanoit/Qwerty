using Qwerty.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qwerty.DAL.Repositories;
using Qwerty.DAL.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace Qwerty.DAL.EF
{
    public class QwertyDbInitializer : DropCreateDatabaseAlways<ApplicationContext>
    {
        protected override void Seed(ApplicationContext context)
        {
            ApplicationUserManager Usermanager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            ApplicationRoleManager Rolemanager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));

            ApplicationRole AdminRole = new ApplicationRole { Name = "admin" };
            ApplicationRole UserRole = new ApplicationRole { Name = "user" };

            Rolemanager.Create(AdminRole);
            Rolemanager.Create(UserRole);

            ApplicationUser Admin = new ApplicationUser()
            {
                Email = "Dimanoit@gmail.com",
                UserName = "Dimanoit"
            };
            string AdminPassword = "qwertyadmin";

            ApplicationUser Anna = new ApplicationUser()
            {
                Email = "Anna@gmail.com",
                UserName = "Anna"
            };
            string AnnaPassword = "1234qwerty";

            User Admin1 = new User()
            {
                ApplicationUser = Admin,
                Login = Admin.UserName,
                Password = AdminPassword,
                RecivedMessages = null,
                ReciveFriendshipRequests = null,
                SendFriendshipRequests = null,
                SendMessages = null,
                Friends = null
            };
            User Anna1 = new User()
            {
                ApplicationUser = Anna,
                Login = Anna.UserName,
                Password = AnnaPassword,
                RecivedMessages = null,
                ReciveFriendshipRequests = null,
                SendFriendshipRequests = null,
                SendMessages = null,
                Friends = null
            };

            

            var result = Usermanager.Create(Admin, AdminPassword);
            var result2 = Usermanager.Create(Anna, AnnaPassword);

            if(result.Succeeded && result2.Succeeded)
            {
                Usermanager.AddToRole(Admin.Id, AdminRole.Name);
                Usermanager.AddToRole(Admin.Id, UserRole.Name);
                Usermanager.AddToRole(Anna.Id, UserRole.Name);
            }
            Anna1.UserId = Anna.Id;
            Admin1.UserId = Admin.Id;

            context.QUsers.Add(Admin1);
            context.QUsers.Add(Anna1);
            context.SaveChanges();

            base.Seed(context);
        }
    }
}
