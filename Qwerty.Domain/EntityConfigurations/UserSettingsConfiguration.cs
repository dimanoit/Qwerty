using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwerty.DAL.Entities;

namespace Qwerty.DAL.EntityConfigurations
{
    internal class UserSettingsConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.UserId);
            builder.ToTable("Users");
            builder.Property(x => x.UserId).ValueGeneratedNever();
            builder.HasOne(x => x.UserProfile).WithOne(x => x.User).HasForeignKey<User>(user => user.UserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}