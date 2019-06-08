using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.Configuration;
using Qwerty.BLL.Interfaces;
using Qwerty.BLL.Services;
using Qwerty.DAL.EF;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Identity;
using Qwerty.DAL.Interfaces;
using Qwerty.DAL.Repositories;

namespace Qwerty.Settings
{
    public static class Settings
    {
        public static void RegistrationServices(this IServiceCollection services, string connectionName)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsetings.json").Build();

            var connectionString = configuration.GetConnectionString(connectionName);
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IFriendService, FriendService>();
            services.AddScoped<IFriendshipRequestService, FriendshipRequestService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IUserService, UserService>();

            services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddUserManager<ApplicationUserManager>()
                .AddRoleManager<ApplicationRoleManager>()
                .AddDefaultTokenProviders();
        }
    }
}
