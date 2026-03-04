namespace AgoraCommerce.Contracts.Basket;

public sealed class BasketDto
{
    public required Guid BasketId { get; init; }

    public Guid? AnonymousId { get; init; }

    public required IReadOnlyList<BasketItemDto> Items { get; init; }

    public decimal Subtotal { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }
}

public sealed class BasketItemDto
{
    public required Guid ProductId { get; init; }

    public required string Name { get; init; }

    public required string Sku { get; init; }

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public required string Currency { get; init; }

    public decimal LineTotal { get; init; }
}

public sealed record AddBasketItemRequest(Guid ProductId, int Quantity);

public sealed record UpdateBasketItemQuantityRequest(int Quantity);
