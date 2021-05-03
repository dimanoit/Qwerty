using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Qwerty.DAL.Entities;
using Qwerty.DAL.EntityConfigurations;

namespace Qwerty.DAL.EF
{
    public sealed class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> QUsers { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<FriendshipRequest> Requests { get; set; }

        public DbSet<UserProfile> Profiles { get; set; }

        public DbSet<Friend> Friends { get; set; }

        public DbSet<UserFriends> UserFriends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assemblyWithConfigurations = typeof(FriendConfiguration).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assemblyWithConfigurations);
            base.OnModelCreating(modelBuilder);
        }
    }
}