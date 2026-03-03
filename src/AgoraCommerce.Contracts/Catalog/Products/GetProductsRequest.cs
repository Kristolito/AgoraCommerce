namespace AgoraCommerce.Contracts.Catalog.Products;

public sealed class GetProductsRequest
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public Guid? CategoryId { get; init; }

    public string? Brand { get; init; }

    public decimal? MinPrice { get; init; }

    public decimal? MaxPrice { get; init; }

    public string? Search { get; init; }

    public string? Sort { get; init; } = "newest";

    public bool? IsActive { get; init; }
}
