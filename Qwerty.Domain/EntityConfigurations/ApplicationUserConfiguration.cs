using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwerty.DAL.Entities;

namespace Qwerty.DAL.EntityConfigurations
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder
                .HasOne(x => x.User)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey<User>(x => x.UserId);
        }
    }
}