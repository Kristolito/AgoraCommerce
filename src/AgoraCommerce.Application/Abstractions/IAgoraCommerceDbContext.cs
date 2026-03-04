using AgoraCommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgoraCommerce.Application.Abstractions;

public interface IAgoraCommerceDbContext
{
    DbSet<Product> Products { get; }

    DbSet<Category> Categories { get; }

    DbSet<Basket> Baskets { get; }

    DbSet<BasketItem> BasketItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
