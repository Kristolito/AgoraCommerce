namespace AgoraCommerce.Contracts.Catalog.Products;

public sealed record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    Guid CategoryId,
    string? Brand);

public sealed record AdminProductDto(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    Guid CategoryId,
    string? Brand,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateProductRequest(
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    string? Currency,
    Guid CategoryId,
    string? Brand);

public sealed record UpdateProductRequest(
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    string? Currency,
    Guid CategoryId,
    string? Brand);
