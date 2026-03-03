using AgoraCommerce.Domain.Entities;
using AgoraCommerce.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AgoraCommerce.Infrastructure.Persistence;

public class AgoraCommerceDbContext(DbContextOptions<AgoraCommerceDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
