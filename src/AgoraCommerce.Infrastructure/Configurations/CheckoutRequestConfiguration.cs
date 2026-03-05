using AgoraCommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgoraCommerce.Infrastructure.Configurations;

public class CheckoutRequestConfiguration : IEntityTypeConfiguration<CheckoutRequest>
{
    public void Configure(EntityTypeBuilder<CheckoutRequest> builder)
    {
        builder.ToTable("CheckoutRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OwnerType)
            .IsRequired();

        builder.Property(x => x.IdempotencyKey)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.ReceivedAt)
            .IsRequired();

        builder.Property(x => x.Error)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => new { x.OwnerType, x.AnonymousId, x.IdempotencyKey })
            .IsUnique();
        builder.HasIndex(x => new { x.OwnerType, x.UserId, x.IdempotencyKey })
            .IsUnique();
    }
}
