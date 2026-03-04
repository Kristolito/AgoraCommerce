using AgoraCommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgoraCommerce.Infrastructure.Configurations;

public class BasketConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.ToTable("Baskets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId);
        builder.Property(x => x.AnonymousId);

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.HasIndex(x => x.AnonymousId);
        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.AnonymousId)
            .IsUnique();

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Basket)
            .HasForeignKey(x => x.BasketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
