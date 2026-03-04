namespace AgoraCommerce.Application.Features.Basket;

public sealed record BasketLineModel(
    Guid ProductId,
    string Name,
    string Sku,
    int Quantity,
    decimal UnitPrice,
    string Currency,
    decimal LineTotal);

public sealed record BasketModel(
    Guid BasketId,
    Guid? AnonymousId,
    IReadOnlyList<BasketLineModel> Items,
    decimal Subtotal,
    DateTimeOffset UpdatedAt);
