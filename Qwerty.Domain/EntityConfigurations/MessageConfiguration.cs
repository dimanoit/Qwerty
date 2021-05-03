using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwerty.DAL.Entities;

namespace Qwerty.DAL.EntityConfigurations
{
    internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");
            builder.HasKey(x => x.IdMessage);
            builder.Property(x => x.IdMessage);

            builder
                .HasOne(x => x.SenderUser)
                .WithMany(x => x.SendMessages)
                .HasForeignKey(x => x.IdSender)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.RecipientUser)
                .WithMany(x => x.ReceivedMessages)
                .HasForeignKey(x => x.IdRecipient)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}