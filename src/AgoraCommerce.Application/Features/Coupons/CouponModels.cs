using AgoraCommerce.Domain.Enums;

namespace AgoraCommerce.Application.Features.Coupons;

public sealed record CouponModel(
    Guid Id,
    string Code,
    CouponType Type,
    decimal Amount,
    string? Currency,
    bool IsActive,
    DateTimeOffset? ActiveFrom,
    DateTimeOffset? ActiveTo,
    int? MaxRedemptions,
    int RedeemedCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CouponValidationModel(
    bool IsValid,
    string? Reason,
    decimal Discount,
    decimal TotalAfterDiscount);
