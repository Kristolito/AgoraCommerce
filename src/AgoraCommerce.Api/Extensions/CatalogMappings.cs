using AgoraCommerce.Contracts.Catalog.Categories;
using AgoraCommerce.Contracts.Catalog.Products;
using AgoraCommerce.Contracts.Basket;
using AgoraCommerce.Contracts.Common;
using AgoraCommerce.Domain.Entities;
using AgoraCommerce.Application.Common.Models;
using AgoraCommerce.Application.Features.Basket;

namespace AgoraCommerce.Api.Extensions;

public static class CatalogMappings
{
    public static CategoryDto ToDto(this Category category) =>
        new(category.Id, category.Name, category.Slug);

    public static AdminCategoryDto ToAdminDto(this Category category) =>
        new(category.Id, category.Name, category.Slug, category.IsActive, category.CreatedAt, category.UpdatedAt);

    public static ProductDto ToDto(this Product product) =>
        new(product.Id, product.Sku, product.Name, product.Description, product.Price, product.Currency, product.CategoryId, product.Brand);

    public static AdminProductDto ToAdminDto(this Product product) =>
        new(product.Id, product.Sku, product.Name, product.Description, product.Price, product.Currency, product.CategoryId, product.Brand, product.IsActive, product.CreatedAt, product.UpdatedAt);

    public static PagedResponse<TDestination> ToPagedResponse<TSource, TDestination>(
        this PagedResult<TSource> result,
        Func<TSource, TDestination> selector) =>
        new()
        {
            Items = result.Items.Select(selector).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            Total = result.Total
        };

    public static BasketDto ToDto(this BasketModel basket) =>
        new()
        {
            BasketId = basket.BasketId,
            AnonymousId = basket.AnonymousId,
            Items = basket.Items.Select(item => new BasketItemDto
            {
                ProductId = item.ProductId,
                Name = item.Name,
                Sku = item.Sku,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Currency = item.Currency,
                LineTotal = item.LineTotal
            }).ToList(),
            Subtotal = basket.Subtotal,
            UpdatedAt = basket.UpdatedAt
        };
}
