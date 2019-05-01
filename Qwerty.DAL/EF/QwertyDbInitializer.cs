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
    public class QwertyDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationContext>
    {
        protected override void Seed(ApplicationContext context)
        {
            ApplicationUserManager Usermanager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            ApplicationRoleManager Rolemanager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));

            ApplicationRole AdminRole = new ApplicationRole { Name = "admin" };
            ApplicationRole UserRole = new ApplicationRole { Name = "user" };
            ApplicationRole BlockedUserRole = new ApplicationRole { Name = "blocked" };
            ApplicationRole TemporarilyDeletedRole = new ApplicationRole { Name = "deleted" };


            Rolemanager.Create(AdminRole);
            Rolemanager.Create(UserRole);
            Rolemanager.Create(BlockedUserRole);
            Rolemanager.Create(TemporarilyDeletedRole);



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
            };

            UserProfile AdminProfile = new UserProfile()
            {
                User = Admin1,
                Email = "Admin@gmail.com",
                Name = "Admin",
                Surname = "AdminSur"
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
            };

            UserProfile AnnaProfile = new UserProfile()
            {
                User = Anna1,
                Email = "Anna@gmail.com",
                Name = "Anna",
                Surname = "AnnaSur"
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
            AnnaProfile.UserId = Anna1.UserId;

            Admin1.UserId = Admin.Id;
            AdminProfile.UserId = Admin1.UserId;

            context.Profiles.Add(AnnaProfile);
            context.Profiles.Add(AdminProfile);

            context.QUsers.Add(Admin1);
            context.QUsers.Add(Anna1);

            Message message = new Message()
            {
                DateAndTimeMessage = DateTime.Now,

                IdRecipient = Admin1.UserId,
                RecipientUser = Admin1,

                IdSender = Anna1.UserId,
                SenderUser = Anna1,

                TextMessage = "First Text Message"
            };

            context.Messages.Add(message);
            context.SaveChanges();

            base.Seed(context);
        }
    }
}
