namespace AgoraCommerce.Contracts.Orders;

public sealed class AddressDto
{
    public required string Line1 { get; init; }

    public string? Line2 { get; init; }

    public required string City { get; init; }

    public required string Postcode { get; init; }

    public required string Country { get; init; }
}

public sealed class OrderLineDto
{
    public Guid ProductId { get; init; }

    public required string ProductName { get; init; }

    public required string Sku { get; init; }

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal LineTotal { get; init; }
}

public sealed class OrderDto
{
    public Guid OrderId { get; init; }

    public required string OrderNumber { get; init; }

    public OrderStatusDto Status { get; init; }

    public decimal Subtotal { get; init; }

    public decimal Discount { get; init; }

    public decimal Total { get; init; }

    public required string Currency { get; init; }

    public required AddressDto ShippingAddress { get; init; }

    public required IReadOnlyList<OrderLineDto> Lines { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
