using System.Net;
using System.Net.Http.Json;
using AgoraCommerce.Contracts.Basket;
using AgoraCommerce.Contracts.Checkout;
using AgoraCommerce.Contracts.Catalog.Categories;
using AgoraCommerce.Contracts.Catalog.Products;
using AgoraCommerce.Contracts.Common;
using AgoraCommerce.Contracts.Orders;
using AgoraCommerce.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace AgoraCommerce.IntegrationTests.Checkout;

public class CheckoutAndOrdersEndpointsTests
{
    [Fact]
    public async Task Checkout_With_Same_Idempotency_Key_Should_Create_Single_Order_And_Clear_Basket()
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

        var product = await CreateProductAsync(client, "SKU-CHK-1", 30m);
        await client.PostAsJsonAsync("/api/v1/basket/items", new AddBasketItemRequest(product.Id, 2));

        var idempotencyKey = Guid.NewGuid().ToString();
        var checkoutRequest = new CheckoutRequest(new CheckoutAddressRequest("Line1", null, "City", "P1", "GB"));

        var first = await SendCheckout(client, idempotencyKey, checkoutRequest);
        var second = await SendCheckout(client, idempotencyKey, checkoutRequest);

        first.StatusCode.Should().Be(HttpStatusCode.Created);
        second.StatusCode.Should().Be(HttpStatusCode.OK);

        var firstBody = await first.Content.ReadFromJsonAsync<CheckoutResponse>();
        var secondBody = await second.Content.ReadFromJsonAsync<CheckoutResponse>();
        firstBody.Should().NotBeNull();
        secondBody.Should().NotBeNull();
        secondBody!.OrderId.Should().Be(firstBody!.OrderId);
        secondBody.OrderNumber.Should().Be(firstBody.OrderNumber);

        var basketResponse = await client.GetAsync("/api/v1/basket");
        var basket = await basketResponse.Content.ReadFromJsonAsync<BasketDto>();
        basket!.Items.Should().BeEmpty();

        var ordersResponse = await client.GetAsync("/api/v1/orders?page=1&pageSize=20");
        ordersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var orders = await ordersResponse.Content.ReadFromJsonAsync<PagedResponse<OrderDto>>();
        orders!.Items.Should().Contain(x => x.OrderId == firstBody.OrderId);
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
        categoryResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var category = await categoryResponse.Content.ReadFromJsonAsync<AdminCategoryDto>();
        category.Should().NotBeNull();

        var productResponse = await client.PostAsJsonAsync(
            "/api/v1/admin/products",
            new CreateProductRequest(
                sku,
                $"Product-{Guid.NewGuid():N}",
                "Checkout test product",
                price,
                "GBP",
                category!.Id,
                "AgoraBrand"));

        productResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var product = await productResponse.Content.ReadFromJsonAsync<AdminProductDto>();
        product.Should().NotBeNull();
        return product!;
    }
}
