using AgoraCommerce.Application.Common.Exceptions;
using AgoraCommerce.Application.Features.Checkout;
using AgoraCommerce.Domain.Entities;
using AgoraCommerce.UnitTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using DomainBasket = AgoraCommerce.Domain.Entities.Basket;

namespace AgoraCommerce.UnitTests.Checkout;

public class CheckoutServiceTests
{
    [Fact]
    public async Task Checkout_Should_Fail_On_Empty_Basket()
    {
        await using var dbContext = CreateDbContext();
        var anonymousId = Guid.NewGuid();
        var basket = DomainBasket.Create(null, anonymousId);
        await dbContext.Baskets.AddAsync(basket);
        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);

        var act = () => service.CheckoutBasketAsync(CreateCommand(anonymousId, "key-empty"));

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Checkout_Should_Create_Order_With_Correct_Totals()
    {
        await using var dbContext = CreateDbContext();
        var anonymousId = Guid.NewGuid();
        var category = Category.Create("Category", "category");
        var product = Product.Create("SKU-1", "Product 1", null, 25m, "GBP", category.Id, "Brand");
        var basket = DomainBasket.Create(null, anonymousId);
        basket.AddOrIncrementItem(product.Id, 2, product.Price, product.Currency);

        await dbContext.Categories.AddAsync(category);
        await dbContext.Products.AddAsync(product);
        await dbContext.Baskets.AddAsync(basket);
        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);
        var result = await service.CheckoutBasketAsync(CreateCommand(anonymousId, "key-totals"));

        result.Subtotal.Should().Be(50m);
        result.Total.Should().Be(50m);
        result.Status.Should().Be(Domain.Enums.OrderStatus.PendingPayment);
        dbContext.Orders.Count().Should().Be(1);
        dbContext.OrderLines.Count().Should().Be(1);
        basket.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Checkout_Should_Be_Idempotent_For_Same_Key()
    {
        await using var dbContext = CreateDbContext();
        var anonymousId = Guid.NewGuid();
        var category = Category.Create("Category", "category");
        var product = Product.Create("SKU-2", "Product 2", null, 10m, "GBP", category.Id, "Brand");
        var basket = DomainBasket.Create(null, anonymousId);
        basket.AddOrIncrementItem(product.Id, 1, product.Price, product.Currency);

        await dbContext.Categories.AddAsync(category);
        await dbContext.Products.AddAsync(product);
        await dbContext.Baskets.AddAsync(basket);
        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);
        var first = await service.CheckoutBasketAsync(CreateCommand(anonymousId, "same-key"));
        var second = await service.CheckoutBasketAsync(CreateCommand(anonymousId, "same-key"));

        first.OrderId.Should().Be(second.OrderId);
        first.OrderNumber.Should().Be(second.OrderNumber);
        second.IsFromIdempotencyReplay.Should().BeTrue();
        dbContext.Orders.Count().Should().Be(1);
    }

    private static TestAgoraCommerceDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestAgoraCommerceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestAgoraCommerceDbContext(options);
    }

    private static CheckoutService CreateService(TestAgoraCommerceDbContext dbContext)
    {
        return new CheckoutService(
            dbContext,
            new TestOrderNumberGenerator(),
            new CheckoutBasketCommandValidator());
    }

    private static CheckoutBasketCommand CreateCommand(Guid anonymousId, string key)
    {
        return new CheckoutBasketCommand(
            null,
            anonymousId,
            key,
            new CheckoutAddressModel("Line1", null, "City", "PC1", "GB"));
    }
}
