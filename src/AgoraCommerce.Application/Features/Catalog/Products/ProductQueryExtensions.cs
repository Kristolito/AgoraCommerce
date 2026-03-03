using AgoraCommerce.Domain.Entities;

namespace AgoraCommerce.Application.Features.Catalog.Products;

public static class ProductQueryExtensions
{
    public static IQueryable<Product> ApplyFilters(this IQueryable<Product> query, ProductListQuery request)
    {
        if (request.CategoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == request.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Brand))
        {
            var brand = request.Brand.Trim();
            query = query.Where(x => x.Brand != null && x.Brand.Contains(brand));
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(x => x.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(x => x.Price <= request.MaxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.Name.Contains(search) || (x.Brand != null && x.Brand.Contains(search)));
        }

        return query;
    }

    public static IQueryable<Product> ApplySort(this IQueryable<Product> query, string? sort)
    {
        return sort?.ToLowerInvariant() switch
        {
            "price_asc" => query.OrderBy(x => x.Price).ThenByDescending(x => x.CreatedAt),
            "price_desc" => query.OrderByDescending(x => x.Price).ThenByDescending(x => x.CreatedAt),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };
    }
}
