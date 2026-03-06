using AgoraCommerce.Domain.Enums;

namespace AgoraCommerce.Application.Features.Coupons;

public sealed record CreateCouponCommand(
    string Code,
    CouponType Type,
    decimal Amount,
    string? Currency,
    bool IsActive,
    DateTimeOffset? ActiveFrom,
    DateTimeOffset? ActiveTo,
    int? MaxRedemptions);

public sealed record UpdateCouponCommand(
    Guid Id,
    CouponType Type,
    decimal Amount,
    string? Currency,
    bool IsActive,
    DateTimeOffset? ActiveFrom,
    DateTimeOffset? ActiveTo,
    int? MaxRedemptions);

public sealed record DeactivateCouponCommand(Guid Id);
