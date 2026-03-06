using AgoraCommerce.Domain.Enums;

namespace AgoraCommerce.Application.Features.Checkout;

public sealed record CheckoutAddressModel(
    string Line1,
    string? Line2,
    string City,
    string Postcode,
    string Country);

public sealed record CheckoutResultModel(
    Guid OrderId,
    string OrderNumber,
    OrderStatus Status,
    decimal Subtotal,
    decimal Discount,
    decimal Total,
    string Currency,
    DateTimeOffset CreatedAt,
    string? CouponCode,
    bool IsFromIdempotencyReplay);
