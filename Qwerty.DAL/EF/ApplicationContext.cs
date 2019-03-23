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
        public ApplicationContext(string ConnectionString) : base(ConnectionString) { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region UserSettings
            modelBuilder.Entity<User>().HasKey(x => x.UserId);
            modelBuilder.Entity<User>().Property(x => x.UserId)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            modelBuilder.Entity<User>().HasRequired(x => x.ApplicationUser).WithRequiredPrincipal(x => x.User);
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
            base.OnModelCreating(modelBuilder);
        }
    }
}
