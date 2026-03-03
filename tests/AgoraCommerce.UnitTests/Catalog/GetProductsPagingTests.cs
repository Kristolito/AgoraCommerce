using AgoraCommerce.Application.Features.Catalog.Products;
using AgoraCommerce.Domain.Entities;
using FluentAssertions;

namespace AgoraCommerce.UnitTests.Catalog;

public class GetProductsPagingTests
{
    [Fact]
    public void Should_Apply_Price_Desc_And_Paging()
    {
        var categoryId = Guid.NewGuid();
        var products = new List<Product>
        {
            Product.Create("SKU-1", "Product 1", null, 10m, "GBP", categoryId, "Brand"),
            Product.Create("SKU-2", "Product 2", null, 20m, "GBP", categoryId, "Brand"),
            Product.Create("SKU-3", "Product 3", null, 30m, "GBP", categoryId, "Brand"),
            Product.Create("SKU-4", "Product 4", null, 40m, "GBP", categoryId, "Brand")
        };

        var query = new ProductListQuery(Page: 2, PageSize: 2, Sort: "price_desc");

        var paged = products.AsQueryable()
            .ApplyFilters(query)
            .ApplySort(query.Sort)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        paged.Select(x => x.Sku).Should().Equal("SKU-2", "SKU-1");
    }
}
