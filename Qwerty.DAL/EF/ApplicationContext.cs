using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwerty.DAL.Entities;

namespace Qwerty.DAL.EF
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        public DbSet<User> QUsers { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<FriendshipRequest> Requests { get; set; }

        public DbSet<UserProfile> Profiles { get; set; }

        public DbSet<Friend> Friends { get; set; }

        public DbSet<UserFriends> UserFriends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region UserSettings
            modelBuilder.Entity<User>().HasKey(x => x.UserId);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>().Property(x => x.UserId).ValueGeneratedNever();
            modelBuilder.Entity<ApplicationUser>().HasOne(x => x.User).WithOne(x => x.ApplicationUser).HasForeignKey<User>(x => x.UserId);
            #endregion

            #region FriendSettings
            modelBuilder.Entity<Friend>().ToTable("Friends");
            modelBuilder.Entity<Friend>().HasKey(x => x.FriendId);
            modelBuilder.Entity<Friend>().HasOne(x => x.UserProfile)
                .WithOne(x => x.ProfileAsFriend).HasForeignKey<Friend>(x => x.FriendId);
            #endregion

            #region Messages
            modelBuilder.Entity<Message>().ToTable("Messages");
            modelBuilder.Entity<Message>().HasKey(x => x.IdMessage);
            modelBuilder.Entity<Message>().Property(x => x.IdMessage);
            modelBuilder.Entity<Message>().HasOne(x => x.SenderUser)
                .WithMany(x => x.SendMessages).HasForeignKey(x => x.IdSender).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Message>().HasOne(x => x.RecipientUser).WithMany(x => x.ReceivedMessages)
                .HasForeignKey(x => x.IdRecipient).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region FriendshipRequest
            modelBuilder.Entity<FriendshipRequest>().ToTable("FriendshipRequests");
            modelBuilder.Entity<FriendshipRequest>().HasKey(x => new { x.SenderUserId, x.RecipientUserId });
            modelBuilder.Entity<FriendshipRequest>().HasOne(x => x.RecipientUser)
                .WithMany(x => x.ReceiveFriendshipRequests)
                .HasForeignKey(x => x.RecipientUserId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FriendshipRequest>().HasOne(x => x.SenderUser).WithMany(x => x.SendFriendshipRequests)
                .HasForeignKey(x => x.SenderUserId).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region UserProfileSettings
            modelBuilder.Entity<UserProfile>().ToTable("UserProfiles");
            modelBuilder.Entity<UserProfile>().HasKey(x => x.UserId);
            modelBuilder.Entity<UserProfile>().HasOne(x => x.User).WithOne(x => x.UserProfile);
            #endregion

            #region UserFriendsSettings
            modelBuilder.Entity<UserFriends>().ToTable("UserFriends");
            modelBuilder.Entity<UserFriends>().HasKey(x => new { x.FriendId, x.UserId });
            modelBuilder.Entity<UserFriends>().HasOne(x => x.User).WithMany(x => x.UserFriends);
            modelBuilder.Entity<UserFriends>().HasOne(x => x.Friend).WithMany(x => x.UserFriends);
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
