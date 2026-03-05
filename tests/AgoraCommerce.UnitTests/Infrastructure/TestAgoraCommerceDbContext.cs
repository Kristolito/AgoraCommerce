using AgoraCommerce.Application.Abstractions;
using AgoraCommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using DomainBasket = AgoraCommerce.Domain.Entities.Basket;

namespace AgoraCommerce.UnitTests.Infrastructure;

public sealed class TestAgoraCommerceDbContext(DbContextOptions<TestAgoraCommerceDbContext> options) : DbContext(options), IAgoraCommerceDbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<DomainBasket> Baskets => Set<DomainBasket>();
    public DbSet<BasketItem> BasketItems => Set<BasketItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<CheckoutRequest> CheckoutRequests => Set<CheckoutRequest>();

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IDbContextTransaction>(new NoOpDbContextTransaction());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(builder =>
        {
            builder.OwnsOne(x => x.ShippingAddress, address =>
            {
                address.Property(x => x.Line1);
                address.Property(x => x.Line2);
                address.Property(x => x.City);
                address.Property(x => x.Postcode);
                address.Property(x => x.Country);
            });

            builder.HasMany(x => x.Lines)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId);
        });

        modelBuilder.Entity<DomainBasket>(builder =>
        {
            builder.HasMany(x => x.Items)
                .WithOne(x => x.Basket)
                .HasForeignKey(x => x.BasketId);
        });

        base.OnModelCreating(modelBuilder);
    }
}

internal sealed class NoOpDbContextTransaction : IDbContextTransaction
{
    public Guid TransactionId { get; } = Guid.NewGuid();

    public void Dispose()
    {
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public void Commit()
    {
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Rollback()
    {
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void CreateSavepoint(string name)
    {
    }

    public Task CreateSavepointAsync(string name, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void ReleaseSavepoint(string name)
    {
    }

    public Task ReleaseSavepointAsync(string name, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void RollbackToSavepoint(string name)
    {
    }

    public Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
