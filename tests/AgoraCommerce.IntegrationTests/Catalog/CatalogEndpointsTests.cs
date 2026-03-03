using System.Net;
using System.Net.Http.Json;
using AgoraCommerce.Contracts.Catalog.Categories;
using AgoraCommerce.Contracts.Catalog.Products;
using AgoraCommerce.Contracts.Common;
using AgoraCommerce.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace AgoraCommerce.IntegrationTests.Catalog;

public class CatalogEndpointsTests
{
    [Fact]
    public async Task Create_Product_Then_Public_GetProducts_Should_Return_It()
    {
        if (!await MySqlTestGuard.IsAvailableAsync())
        {
            return;
        }

        await using var factory = new CatalogApiFactory();
        await factory.InitializeDatabaseAsync();
        var client = factory.CreateClient();

        var categoryResponse = await client.PostAsJsonAsync("/api/v1/admin/categories", new CreateCategoryRequest("Electronics", "electronics"));
        categoryResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var category = await categoryResponse.Content.ReadFromJsonAsync<AdminCategoryDto>();
        category.Should().NotBeNull();

        var createProduct = new CreateProductRequest("SKU-CAT-1", "Laptop", "High-end laptop", 1499.99m, "GBP", category!.Id, "AgoraBrand");
        var productResponse = await client.PostAsJsonAsync("/api/v1/admin/products", createProduct);
        productResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var product = await productResponse.Content.ReadFromJsonAsync<AdminProductDto>();
        product.Should().NotBeNull();

        var listResponse = await client.GetAsync("/api/v1/products?page=1&pageSize=20");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var paged = await listResponse.Content.ReadFromJsonAsync<PagedResponse<ProductDto>>();
        paged.Should().NotBeNull();
        paged!.Items.Should().Contain(x => x.Id == product!.Id);
    }

    [Fact]
    public async Task Soft_Deleted_Product_Should_Be_Hidden_Public_But_Visible_Admin()
    {
        if (!await MySqlTestGuard.IsAvailableAsync())
        {
            return;
        }

        await using var factory = new CatalogApiFactory();
        await factory.InitializeDatabaseAsync();
        var client = factory.CreateClient();

        var categoryResponse = await client.PostAsJsonAsync("/api/v1/admin/categories", new CreateCategoryRequest("Home", "home"));
        var category = await categoryResponse.Content.ReadFromJsonAsync<AdminCategoryDto>();

        var productResponse = await client.PostAsJsonAsync(
            "/api/v1/admin/products",
            new CreateProductRequest("SKU-CAT-2", "Chair", "Office chair", 120m, "GBP", category!.Id, "AgoraBrand"));
        var product = await productResponse.Content.ReadFromJsonAsync<AdminProductDto>();

        var deleteResponse = await client.DeleteAsync($"/api/v1/admin/products/{product!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var publicResponse = await client.GetAsync("/api/v1/products?page=1&pageSize=20");
        var publicPaged = await publicResponse.Content.ReadFromJsonAsync<PagedResponse<ProductDto>>();
        publicPaged!.Items.Should().NotContain(x => x.Id == product.Id);

        var adminResponse = await client.GetAsync("/api/v1/admin/products?page=1&pageSize=20&isActive=false");
        var adminPaged = await adminResponse.Content.ReadFromJsonAsync<PagedResponse<AdminProductDto>>();
        adminPaged!.Items.Should().Contain(x => x.Id == product.Id && !x.IsActive);
    }
}
