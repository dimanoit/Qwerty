using Microsoft.AspNet.Identity.EntityFramework;
using Qwerty.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.DAL.EF
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(string ConnectionString) : base(ConnectionString)
        {
            Database.SetInitializer<ApplicationContext>(new QwertyDbInitializer());
        }
        public DbSet<User> QUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FriendshipRequest> Requests { get; set; }
        public DbSet<UserProfile> Profiles { get; set; }
        public DbSet<Friend> Friends { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region UserSettings
            modelBuilder.Entity<User>().HasKey(x => x.UserId);
            modelBuilder.Entity<User>().Property(x => x.UserId)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            modelBuilder.Entity<ApplicationUser>().HasRequired(x => x.User).WithRequiredPrincipal(x => x.ApplicationUser);
            modelBuilder.Entity<User>().HasMany(x => x.Friends).WithMany(x => x.Users).Map(m =>
            {
                m.MapLeftKey("UserId");
                m.MapRightKey("FriendId");
                m.ToTable("UsersFriends");
            });
            #endregion
            #region FriendSettings
            modelBuilder.Entity<Friend>().HasKey(x => x.FriendId);
            modelBuilder.Entity<Friend>().HasRequired(x => x.UserProfile).WithRequiredPrincipal(x => x.ProfileAsFriend);
            #endregion
            #region Messages
            modelBuilder.Entity<Message>().HasKey(x => x.IdMessage);
            modelBuilder.Entity<Message>().Property(x => x.IdMessage).
                HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Message>().HasRequired(x => x.SenderUser).WithMany(x => x.SendMessages).HasForeignKey(x => x.IdSender).WillCascadeOnDelete(false);
            modelBuilder.Entity<Message>().HasRequired(x => x.RecipientUser).WithMany(x => x.RecivedMessages).HasForeignKey(x => x.IdRecipient).WillCascadeOnDelete(false);
            #endregion
            #region FriendshipRequest
            modelBuilder.Entity<FriendshipRequest>().HasKey(x => new { x.SenderUserId, x.RecipientUserId });
            modelBuilder.Entity<FriendshipRequest>().HasRequired(x => x.RecipientUser).WithMany(x => x.ReciveFriendshipRequests)
                .HasForeignKey(x => x.RecipientUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<FriendshipRequest>().HasRequired(x => x.SenderUser).WithMany(x => x.SendFriendshipRequests)
                .HasForeignKey(x => x.SenderUserId).WillCascadeOnDelete(false);
            #endregion
            #region UserProfileSettings
            modelBuilder.Entity<UserProfile>().HasKey(x => x.UserId);
            #endregion
            base.OnModelCreating(modelBuilder);
        }
    }
}
