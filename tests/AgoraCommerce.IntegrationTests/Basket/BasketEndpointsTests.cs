using System.Net;
using System.Net.Http.Json;
using AgoraCommerce.Contracts.Basket;
using AgoraCommerce.Contracts.Catalog.Categories;
using AgoraCommerce.Contracts.Catalog.Products;
using AgoraCommerce.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace AgoraCommerce.IntegrationTests.Basket;

public class BasketEndpointsTests
{
    [Fact]
    public async Task Get_Basket_With_New_Anonymous_Id_Should_Return_Empty_Basket()
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

        var response = await client.GetAsync("/api/v1/basket");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.TryGetValues("X-Anonymous-Id", out var values).Should().BeTrue();
        values!.Single().Should().Be(anonymousId);

        var basket = await response.Content.ReadFromJsonAsync<BasketDto>();
        basket.Should().NotBeNull();
        basket!.Items.Should().BeEmpty();
        basket.Subtotal.Should().Be(0m);
    }

    [Fact]
    public async Task Add_Item_Then_Get_Basket_Should_Return_Item_And_Subtotal()
    {
        if (!await MySqlTestGuard.IsAvailableAsync())
        {
            return;
        }

        await using var factory = new CatalogApiFactory();
        await factory.InitializeDatabaseAsync();
        var client = factory.CreateClient();

        var product = await CreateProductAsync(client, "SKU-BASKET-1", 25m);
        var anonymousId = Guid.NewGuid().ToString();
        client.DefaultRequestHeaders.Add("X-Anonymous-Id", anonymousId);

        var addResponse = await client.PostAsJsonAsync("/api/v1/basket/items", new AddBasketItemRequest(product.Id, 2));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await client.GetAsync("/api/v1/basket");
        var basket = await getResponse.Content.ReadFromJsonAsync<BasketDto>();
        basket.Should().NotBeNull();
        basket!.Items.Should().ContainSingle();
        basket.Items[0].Quantity.Should().Be(2);
        basket.Items[0].UnitPrice.Should().Be(25m);
        basket.Subtotal.Should().Be(50m);
    }

    [Fact]
    public async Task Adding_Same_Item_Twice_Should_Result_In_Single_Line_With_Summed_Quantity()
    {
        if (!await MySqlTestGuard.IsAvailableAsync())
        {
            return;
        }

        await using var factory = new CatalogApiFactory();
        await factory.InitializeDatabaseAsync();
        var client = factory.CreateClient();

        var product = await CreateProductAsync(client, "SKU-BASKET-2", 15m);
        var anonymousId = Guid.NewGuid().ToString();
        client.DefaultRequestHeaders.Add("X-Anonymous-Id", anonymousId);

        await client.PostAsJsonAsync("/api/v1/basket/items", new AddBasketItemRequest(product.Id, 1));
        await client.PostAsJsonAsync("/api/v1/basket/items", new AddBasketItemRequest(product.Id, 3));

        var response = await client.GetAsync("/api/v1/basket");
        var basket = await response.Content.ReadFromJsonAsync<BasketDto>();
        basket.Should().NotBeNull();
        basket!.Items.Should().ContainSingle();
        basket.Items[0].Quantity.Should().Be(4);
        basket.Subtotal.Should().Be(60m);
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
                "Basket test product",
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
