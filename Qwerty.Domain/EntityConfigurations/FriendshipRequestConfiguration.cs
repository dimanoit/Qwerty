using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwerty.DAL.Entities;

namespace Qwerty.DAL.EntityConfigurations
{
    internal class FriendshipRequestConfiguration : IEntityTypeConfiguration<FriendshipRequest>
    {
        public void Configure(EntityTypeBuilder<FriendshipRequest> builder)
        {
            builder.ToTable("FriendshipRequests");
            builder.HasKey(x => new {x.SenderUserId, x.RecipientUserId});

            builder
                .HasOne(x => x.RecipientUser)
                .WithMany(x => x.ReceiveFriendshipRequests)
                .HasForeignKey(x => x.RecipientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.SenderUser)
                .WithMany(x => x.SendFriendshipRequests)
                .HasForeignKey(x => x.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}