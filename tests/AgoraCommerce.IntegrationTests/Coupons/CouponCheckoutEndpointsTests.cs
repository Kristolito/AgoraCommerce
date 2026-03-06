using System.Net;
using System.Net.Http.Json;
using AgoraCommerce.Contracts.Basket;
using AgoraCommerce.Contracts.Checkout;
using AgoraCommerce.Contracts.Coupons;
using AgoraCommerce.Contracts.Catalog.Categories;
using AgoraCommerce.Contracts.Catalog.Products;
using AgoraCommerce.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace AgoraCommerce.IntegrationTests.Coupons;

public class CouponCheckoutEndpointsTests
{
    [Fact]
    public async Task Validate_And_Checkout_With_Coupon_Should_Apply_Discount_And_Not_DoubleRedeem_On_Idempotency_Retry()
    {
        if (!await MySqlTestGuard.IsAvailableAsync())
        {
            return;
        }

        await using var factory = new CatalogApiFactory();
        await factory.InitializeDatabaseAsync();
        var client = factory.CreateClient();
        var anonymousId = Guid.NewGuid().ToString();
        client.DefaultRequestHeaders.Add("X-Anonymous-Id", anonymousId);

        var product = await CreateProductAsync(client, "SKU-CPN-1", 100m);
        var coupon = await CreateCouponAsync(client, "SAVE10", CouponTypeDto.Percent, 10m, true, null, null, null);

        var validateResponse = await client.PostAsJsonAsync("/api/v1/coupons/validate", new ValidateCouponRequest("SAVE10", 100m));
        validateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var validated = await validateResponse.Content.ReadFromJsonAsync<ValidateCouponResponse>();
        validated!.IsValid.Should().BeTrue();
        validated.Discount.Should().Be(10m);
        validated.TotalAfterDiscount.Should().Be(90m);

        await client.PostAsJsonAsync("/api/v1/basket/items", new AddBasketItemRequest(product.Id, 1));
        var body = new CheckoutRequest(new CheckoutAddressRequest("Line1", null, "City", "P1", "GB"), "SAVE10");
        var key = Guid.NewGuid().ToString();

        var first = await SendCheckout(client, key, body);
        var second = await SendCheckout(client, key, body);

        first.StatusCode.Should().Be(HttpStatusCode.Created);
        second.StatusCode.Should().Be(HttpStatusCode.OK);

        var firstOrder = await first.Content.ReadFromJsonAsync<CheckoutResponse>();
        var secondOrder = await second.Content.ReadFromJsonAsync<CheckoutResponse>();
        firstOrder!.OrderId.Should().Be(secondOrder!.OrderId);
        firstOrder.Subtotal.Should().Be(100m);
        firstOrder.Discount.Should().Be(10m);
        firstOrder.Total.Should().Be(90m);
        firstOrder.CouponCode.Should().Be("SAVE10");

        var couponAfter = await client.GetFromJsonAsync<CouponDto>($"/api/v1/admin/coupons/{coupon.Id}");
        couponAfter!.RedeemedCount.Should().Be(1);
    }

    [Fact]
    public async Task Inactive_Coupon_Should_Be_Rejected_At_Checkout()
    {
        if (!await MySqlTestGuard.IsAvailableAsync())
        {
            return;
        }

        await using var factory = new CatalogApiFactory();
        await factory.InitializeDatabaseAsync();
        var client = factory.CreateClient();
        var anonymousId = Guid.NewGuid().ToString();
        client.DefaultRequestHeaders.Add("X-Anonymous-Id", anonymousId);

        var product = await CreateProductAsync(client, "SKU-CPN-2", 50m);
        await CreateCouponAsync(client, "OFFLINE", CouponTypeDto.FixedAmount, 5m, false, null, null, null);
        await client.PostAsJsonAsync("/api/v1/basket/items", new AddBasketItemRequest(product.Id, 1));

        var response = await SendCheckout(
            client,
            Guid.NewGuid().ToString(),
            new CheckoutRequest(new CheckoutAddressRequest("Line1", null, "City", "P1", "GB"), "OFFLINE"));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private static async Task<HttpResponseMessage> SendCheckout(HttpClient client, string idempotencyKey, CheckoutRequest request)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/checkout")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("Idempotency-Key", idempotencyKey);
        return await client.SendAsync(message);
    }

    private static async Task<AdminProductDto> CreateProductAsync(HttpClient client, string sku, decimal price)
    {
        var categoryResponse = await client.PostAsJsonAsync(
            "/api/v1/admin/categories",
            new CreateCategoryRequest($"Category-{Guid.NewGuid():N}", $"category-{Guid.NewGuid():N}"));
        var category = await categoryResponse.Content.ReadFromJsonAsync<AdminCategoryDto>();

        var productResponse = await client.PostAsJsonAsync(
            "/api/v1/admin/products",
            new CreateProductRequest(
                sku,
                $"Product-{Guid.NewGuid():N}",
                "Coupon test product",
                price,
                "GBP",
                category!.Id,
                "AgoraBrand"));

        var product = await productResponse.Content.ReadFromJsonAsync<AdminProductDto>();
        return product!;
    }

    private static async Task<CouponDto> CreateCouponAsync(
        HttpClient client,
        string code,
        CouponTypeDto type,
        decimal amount,
        bool isActive,
        DateTimeOffset? activeFrom,
        DateTimeOffset? activeTo,
        int? maxRedemptions)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/coupons",
            new CreateCouponRequest(code, type, amount, "GBP", isActive, activeFrom, activeTo, maxRedemptions));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var coupon = await response.Content.ReadFromJsonAsync<CouponDto>();
        return coupon!;
    }
}
