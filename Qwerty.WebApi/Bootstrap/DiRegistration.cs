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
using Qwerty.WebApi.InMemoryCache;
using Qwerty.WebApi.InMemoryCache.Interfaces;

namespace Qwerty.WebApi.Bootstrap
{
    public static class DiRegistration
    {
        public static void RegistrationServices(this IServiceCollection services, string connectionName)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            var connectionString = configuration.GetConnectionString(connectionName);
            services.AddDbContext<ApplicationContext>(
                options =>
                {
                    options.UseSqlServer(connectionString);
                    options.UseLazyLoadingProxies();
                });
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IFriendService, FriendService>();
            services.AddScoped<IFriendshipRequestService, FriendshipRequestService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICacheManager, CacheManager>();
            services.AddScoped<IUserConnectionsManager, UserConnectionsManager>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddUserManager<ApplicationUserManager>()
                .AddRoleManager<ApplicationRoleManager>();
        }
    }
}