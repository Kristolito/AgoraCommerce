using AgoraCommerce.Domain.Entities;
using FluentAssertions;
using DomainBasket = AgoraCommerce.Domain.Entities.Basket;

namespace AgoraCommerce.UnitTests.Basket;

public class BasketBehaviorTests
{
    [Fact]
    public void Add_Item_Should_Create_Line()
    {
        var basket = DomainBasket.Create(null, Guid.NewGuid());
        var productId = Guid.NewGuid();

        basket.AddOrIncrementItem(productId, 2, 10m, "GBP");

        basket.Items.Should().HaveCount(1);
        basket.Items[0].ProductId.Should().Be(productId);
        basket.Items[0].Quantity.Should().Be(2);
    }

    [Fact]
    public void Add_Same_Product_Twice_Should_Increment_Quantity()
    {
        var basket = DomainBasket.Create(null, Guid.NewGuid());
        var productId = Guid.NewGuid();

        basket.AddOrIncrementItem(productId, 1, 10m, "GBP");
        basket.AddOrIncrementItem(productId, 3, 10m, "GBP");

        basket.Items.Should().HaveCount(1);
        basket.Items[0].Quantity.Should().Be(4);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Update_Quantity_Should_Reject_Non_Positive(int quantity)
    {
        var basket = DomainBasket.Create(null, Guid.NewGuid());
        var productId = Guid.NewGuid();
        basket.AddOrIncrementItem(productId, 1, 10m, "GBP");

        var act = () => basket.UpdateItemQuantity(productId, quantity);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Remove_Item_Should_Remove_Line()
    {
        var basket = DomainBasket.Create(null, Guid.NewGuid());
        var productId = Guid.NewGuid();
        basket.AddOrIncrementItem(productId, 1, 10m, "GBP");

        basket.RemoveItem(productId);

        basket.Items.Should().BeEmpty();
    }
}
