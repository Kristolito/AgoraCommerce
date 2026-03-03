using AgoraCommerce.Contracts.Catalog.Categories;
using AgoraCommerce.Contracts.Catalog.Products;
using AgoraCommerce.Contracts.Common;
using AgoraCommerce.Domain.Entities;
using AgoraCommerce.Application.Common.Models;

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
}
