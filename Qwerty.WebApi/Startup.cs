using System;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Qwerty.BLL.DTO;
using Qwerty.WEB.Models;
using Qwerty.WebApi.Bootstrap;
using Qwerty.WebApi.Configurations;
using Qwerty.WebApi.HubConfig;
using Qwerty.WebApi.Middlewares;
using Serilog;
using Serilog.Events;

namespace Qwerty.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegistrationServices("DefaultConnection");
            
            services.AddSignalR();
            services.AddMemoryCache();
            
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<MapperSetting>();
                cfg.CreateMap<MessageDTO, MessageViewModel>().ReverseMap();
                cfg.CreateMap<FriendshipRequestViewModel, FriendshipRequestDTO>().ReverseMap();
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                });

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));
          

            services.AddControllers();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.Console()
                .CreateLogger();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCors("MyPolicy");

            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationHub>("/notification-message");
            });

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
