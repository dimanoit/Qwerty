using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwerty.DAL.Entities;

namespace Qwerty.DAL.EntityConfigurations
{
    internal class FriendConfiguration : IEntityTypeConfiguration<Friend>
    {
        public void Configure(EntityTypeBuilder<Friend> builder)
        {
            builder.ToTable("Friends");
            builder.HasKey(x => x.FriendId);

            builder
                .HasOne(x => x.UserProfile)
                .WithOne(x => x.ProfileAsFriend)
                .HasForeignKey<Friend>(x => x.FriendId);
        }
    }
}