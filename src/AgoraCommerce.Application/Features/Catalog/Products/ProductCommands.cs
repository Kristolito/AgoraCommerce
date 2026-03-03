namespace AgoraCommerce.Application.Features.Catalog.Products;

public sealed record CreateProductCommand(
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    string? Currency,
    Guid CategoryId,
    string? Brand);

public sealed record UpdateProductCommand(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    string? Currency,
    Guid CategoryId,
    string? Brand);
