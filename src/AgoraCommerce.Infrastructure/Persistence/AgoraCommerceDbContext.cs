using AgoraCommerce.Application.Abstractions;
using AgoraCommerce.Domain.Entities;
using AgoraCommerce.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AgoraCommerce.Infrastructure.Persistence;

public class AgoraCommerceDbContext(DbContextOptions<AgoraCommerceDbContext> options) : DbContext(options), IAgoraCommerceDbContext
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Basket> Baskets => Set<Basket>();

    public DbSet<BasketItem> BasketItems => Set<BasketItem>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    public DbSet<CheckoutRequest> CheckoutRequests => Set<CheckoutRequest>();

    public DbSet<Coupon> Coupons => Set<Coupon>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new BasketConfiguration());
        modelBuilder.ApplyConfiguration(new BasketItemConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderLineConfiguration());
        modelBuilder.ApplyConfiguration(new CheckoutRequestConfiguration());
        modelBuilder.ApplyConfiguration(new CouponConfiguration());

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

        foreach (var entry in ChangeTracker.Entries<Basket>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.Touch();
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }
}
