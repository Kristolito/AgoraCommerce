using AgoraCommerce.Domain.Enums;

namespace AgoraCommerce.Application.Features.Orders;

public sealed record AddressModel(
    string Line1,
    string? Line2,
    string City,
    string Postcode,
    string Country);

public sealed record OrderLineModel(
    Guid ProductId,
    string ProductName,
    string Sku,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);

public sealed record OrderModel(
    Guid OrderId,
    string OrderNumber,
    OrderStatus Status,
    decimal Subtotal,
    decimal Discount,
    decimal Total,
    string Currency,
    string? CouponCode,
    AddressModel ShippingAddress,
    IReadOnlyList<OrderLineModel> Lines,
    DateTimeOffset CreatedAt);
