using AgoraCommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgoraCommerce.Application.Abstractions;

public interface IAgoraCommerceDbContext
{
    DbSet<Product> Products { get; }

    DbSet<Category> Categories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
