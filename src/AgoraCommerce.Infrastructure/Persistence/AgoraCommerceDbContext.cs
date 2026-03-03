using AgoraCommerce.Application.Abstractions;
using AgoraCommerce.Domain.Entities;
using AgoraCommerce.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AgoraCommerce.Infrastructure.Persistence;

public class AgoraCommerceDbContext(DbContextOptions<AgoraCommerceDbContext> options) : DbContext(options), IAgoraCommerceDbContext
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Category>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Touch(now);
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.Touch(now);
            }
        }

        foreach (var entry in ChangeTracker.Entries<Product>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Touch(now);
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.Touch(now);
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
