namespace AgoraCommerce.Application.Features.Catalog.Products;

public sealed record ProductListQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? CategoryId = null,
    string? Brand = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? Search = null,
    string? Sort = null,
    bool? IsActive = null);
