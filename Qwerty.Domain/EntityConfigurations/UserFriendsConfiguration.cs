using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwerty.DAL.Entities;

namespace Qwerty.DAL.EntityConfigurations
{
    internal class UserFriendsConfiguration : IEntityTypeConfiguration<UserFriends>
    {
        public void Configure(EntityTypeBuilder<UserFriends> builder)
        {
            builder.ToTable("UserFriends");
            builder.HasKey(x => new {x.FriendId, x.UserId});
            builder.HasOne(x => x.User).WithMany(x => x.UserFriends);
            builder.HasOne(x => x.Friend).WithMany(x => x.UserFriends);
        }
    }
}