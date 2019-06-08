using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qwerty.BLL.Interfaces;
using Qwerty.BLL.Services;
using Qwerty.DAL.EF;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Identity;
using Qwerty.DAL.Interfaces;
using Qwerty.DAL.Repositories;

namespace Qwerty.EnvironmentSettings
{
    public static class DiRegistration
    {
        public static void RegistrationServices(this IServiceCollection services, string connectionName)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var connectionString = configuration.GetConnectionString(connectionName);
            services.AddDbContext<ApplicationContext>(
                options =>
                {
                    options.UseSqlServer(connectionString);
                    options.UseLazyLoadingProxies();
                });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IFriendService, FriendService>();
            services.AddScoped<IFriendshipRequestService, FriendshipRequestService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IUserService, UserService>();

            services.AddIdentity<ApplicationUser, IdentityRole>(
                opts =>
                {
                    opts.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddUserManager<ApplicationUserManager>()
                .AddRoleManager<ApplicationRoleManager>();
        }
    }
}
