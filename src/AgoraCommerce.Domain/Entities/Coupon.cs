using AgoraCommerce.Domain.Enums;

namespace AgoraCommerce.Domain.Entities;

public class Coupon
{
    private Coupon()
    {
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public CouponType Type { get; private set; }

    public decimal Amount { get; private set; }

    public string? Currency { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset? ActiveFrom { get; private set; }

    public DateTimeOffset? ActiveTo { get; private set; }

    public int? MaxRedemptions { get; private set; }

    public int RedeemedCount { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Coupon Create(
        string code,
        CouponType type,
        decimal amount,
        string? currency,
        bool isActive,
        DateTimeOffset? activeFrom,
        DateTimeOffset? activeTo,
        int? maxRedemptions)
    {
        ValidateAmount(type, amount);

        var now = DateTimeOffset.UtcNow;
        return new Coupon
        {
            Id = Guid.NewGuid(),
            Code = NormalizeCode(code),
            Type = type,
            Amount = amount,
            Currency = NormalizeCurrency(currency, type),
            IsActive = isActive,
            ActiveFrom = activeFrom,
            ActiveTo = activeTo,
            MaxRedemptions = maxRedemptions,
            RedeemedCount = 0,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Update(
        CouponType type,
        decimal amount,
        string? currency,
        bool isActive,
        DateTimeOffset? activeFrom,
        DateTimeOffset? activeTo,
        int? maxRedemptions)
    {
        ValidateAmount(type, amount);
        Type = type;
        Amount = amount;
        Currency = NormalizeCurrency(currency, type);
        IsActive = isActive;
        ActiveFrom = activeFrom;
        ActiveTo = activeTo;
        MaxRedemptions = maxRedemptions;
        Touch(DateTimeOffset.UtcNow);
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch(DateTimeOffset.UtcNow);
    }

    public void IncrementRedemption()
    {
        RedeemedCount++;
        Touch(DateTimeOffset.UtcNow);
    }

    public bool IsValidAt(DateTimeOffset now)
    {
        if (!IsActive)
        {
            return false;
        }

        if (ActiveFrom.HasValue && now < ActiveFrom.Value)
        {
            return false;
        }

        if (ActiveTo.HasValue && now > ActiveTo.Value)
        {
            return false;
        }

        if (MaxRedemptions.HasValue && RedeemedCount >= MaxRedemptions.Value)
        {
            return false;
        }

        return true;
    }

    public decimal CalculateDiscount(decimal subtotal)
    {
        if (subtotal <= 0)
        {
            return 0;
        }

        var discount = Type switch
        {
            CouponType.Percent => subtotal * (Amount / 100m),
            CouponType.FixedAmount => Amount,
            _ => 0
        };

        return discount > subtotal ? subtotal : discount;
    }

    public void Touch(DateTimeOffset updatedAt)
    {
        UpdatedAt = updatedAt;
    }

    private static string NormalizeCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Coupon code is required.", nameof(value));
        }

        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeCurrency(string? value, CouponType type)
    {
        if (type == CouponType.Percent)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return "GBP";
        }

        return value.Trim().ToUpperInvariant();
    }

    private static void ValidateAmount(CouponType type, decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Coupon amount must be > 0.");
        }

        if (type == CouponType.Percent && amount > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Percent coupon amount must be <= 100.");
        }
    }
}
