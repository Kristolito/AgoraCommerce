using AgoraCommerce.Domain.Entities;
using AgoraCommerce.Domain.Enums;
using FluentAssertions;

namespace AgoraCommerce.UnitTests.Coupons;

public class CouponRulesTests
{
    [Fact]
    public void Percent_Coupon_Should_Calculate_Correct_Discount()
    {
        var coupon = Coupon.Create("SAVE10", CouponType.Percent, 10m, null, true, null, null, null);
        coupon.CalculateDiscount(200m).Should().Be(20m);
    }

    [Fact]
    public void Fixed_Coupon_Should_Calculate_Correct_Discount()
    {
        var coupon = Coupon.Create("TENOFF", CouponType.FixedAmount, 10m, "GBP", true, null, null, null);
        coupon.CalculateDiscount(200m).Should().Be(10m);
    }

    [Fact]
    public void Discount_Should_Not_Reduce_Total_Below_Zero()
    {
        var coupon = Coupon.Create("BIGOFF", CouponType.FixedAmount, 100m, "GBP", true, null, null, null);
        coupon.CalculateDiscount(20m).Should().Be(20m);
    }

    [Fact]
    public void Expired_Coupon_Should_Be_Invalid()
    {
        var coupon = Coupon.Create(
            "OLD",
            CouponType.Percent,
            10m,
            null,
            true,
            DateTimeOffset.UtcNow.AddDays(-10),
            DateTimeOffset.UtcNow.AddDays(-1),
            null);

        coupon.IsValidAt(DateTimeOffset.UtcNow).Should().BeFalse();
    }

    [Fact]
    public void Max_Redemptions_Reached_Should_Be_Invalid()
    {
        var coupon = Coupon.Create("LIMIT1", CouponType.FixedAmount, 5m, "GBP", true, null, null, 1);
        coupon.IncrementRedemption();
        coupon.IsValidAt(DateTimeOffset.UtcNow).Should().BeFalse();
    }
}
