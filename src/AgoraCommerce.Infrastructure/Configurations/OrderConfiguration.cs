using AgoraCommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgoraCommerce.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(x => x.CouponCode)
            .HasMaxLength(64);

        builder.Property(x => x.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(x => x.Discount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Total)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.OrderNumber)
            .IsUnique();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.AnonymousId);

        builder.OwnsOne(x => x.ShippingAddress, address =>
        {
            address.Property(x => x.Line1)
                .IsRequired()
                .HasMaxLength(200);

            address.Property(x => x.Line2)
                .HasMaxLength(200);

            address.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            address.Property(x => x.Postcode)
                .IsRequired()
                .HasMaxLength(20);

            address.Property(x => x.Country)
                .IsRequired()
                .HasMaxLength(2);
        });

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
