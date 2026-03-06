namespace AgoraCommerce.Contracts.Coupons;

public sealed class CouponDto
{
    public Guid Id { get; init; }

    public required string Code { get; init; }

    public CouponTypeDto Type { get; init; }

    public decimal Amount { get; init; }

    public string? Currency { get; init; }

    public bool IsActive { get; init; }

    public DateTimeOffset? ActiveFrom { get; init; }

    public DateTimeOffset? ActiveTo { get; init; }

    public int? MaxRedemptions { get; init; }

    public int RedeemedCount { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }
}

public sealed record CreateCouponRequest(
    string Code,
    CouponTypeDto Type,
    decimal Amount,
    string? Currency,
    bool IsActive,
    DateTimeOffset? ActiveFrom,
    DateTimeOffset? ActiveTo,
    int? MaxRedemptions);

public sealed record UpdateCouponRequest(
    CouponTypeDto Type,
    decimal Amount,
    string? Currency,
    bool IsActive,
    DateTimeOffset? ActiveFrom,
    DateTimeOffset? ActiveTo,
    int? MaxRedemptions);

public sealed record ValidateCouponRequest(string Code, decimal Subtotal);

public sealed class ValidateCouponResponse
{
    public bool IsValid { get; init; }

    public string? Reason { get; init; }

    public decimal Discount { get; init; }

    public decimal TotalAfterDiscount { get; init; }
}
