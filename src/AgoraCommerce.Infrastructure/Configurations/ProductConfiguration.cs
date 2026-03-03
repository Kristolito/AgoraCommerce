using AgoraCommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgoraCommerce.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(product => product.Sku)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(product => product.Description)
            .HasMaxLength(4000);

        builder.Property(product => product.Price)
            .HasPrecision(18, 2);

        builder.Property(product => product.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(product => product.CategoryId)
            .IsRequired();

        builder.Property(product => product.Brand)
            .HasMaxLength(200);

        builder.Property(product => product.IsActive)
            .IsRequired();

        builder.Property(product => product.CreatedAt)
            .IsRequired();

        builder.Property(product => product.UpdatedAt)
            .IsRequired();

        builder.HasIndex(product => product.Sku)
            .IsUnique();

        builder.HasIndex(product => product.CategoryId);

        builder.HasOne(product => product.Category)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
