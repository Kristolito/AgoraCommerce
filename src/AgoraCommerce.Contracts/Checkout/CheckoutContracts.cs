using AgoraCommerce.Contracts.Orders;

namespace AgoraCommerce.Contracts.Checkout;

public sealed record CheckoutAddressRequest(
    string Line1,
    string? Line2,
    string City,
    string Postcode,
    string Country);

public sealed record CheckoutRequest(CheckoutAddressRequest ShippingAddress, string? CouponCode);

public sealed class CheckoutResponse
{
    public required Guid OrderId { get; init; }

    public required string OrderNumber { get; init; }

    public required OrderStatusDto Status { get; init; }

    public decimal Subtotal { get; init; }

    public decimal Discount { get; init; }

    public decimal Total { get; init; }

    public required string Currency { get; init; }

    public string? CouponCode { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
