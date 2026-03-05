using AgoraCommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AgoraCommerce.Application.Abstractions;

public interface IAgoraCommerceDbContext
{
    DbSet<Product> Products { get; }

    DbSet<Category> Categories { get; }

    DbSet<Basket> Baskets { get; }

    DbSet<BasketItem> BasketItems { get; }

    DbSet<Order> Orders { get; }

    DbSet<OrderLine> OrderLines { get; }

    DbSet<CheckoutRequest> CheckoutRequests { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
