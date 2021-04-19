using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Qwerty.DAL.EF;
using Serilog;

namespace Qwerty.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHost host;
            try
            {
                host = CreateWebHostBuilder(args).Build();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            //ATTENTION POST-MIGRATION DB UPDATE
            StartPostMigrationUpdate(host);
            
            host.Run();
        }
        
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();

        private static void StartPostMigrationUpdate(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationContext>();
                DbInitializer.SeedData(context);
            }
        }
    }
}
