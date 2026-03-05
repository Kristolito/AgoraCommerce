namespace AgoraCommerce.Application.Features.Checkout;

public sealed record CheckoutBasketCommand(
    Guid? UserId,
    Guid? AnonymousId,
    string IdempotencyKey,
    CheckoutAddressModel ShippingAddress);
