using AgoraCommerce.Application.Abstractions;
using AgoraCommerce.Application.Common.Exceptions;
using AgoraCommerce.Application.Common.Models;
using AgoraCommerce.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AgoraCommerce.Application.Features.Catalog.Products;

public interface IProductService
{
    Task<Product> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken = default);

    Task<Product> UpdateProductAsync(UpdateProductCommand command, CancellationToken cancellationToken = default);

    Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Product> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Product> AdminGetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> GetProductsAsync(ProductListQuery query, CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> AdminGetProductsAsync(ProductListQuery query, CancellationToken cancellationToken = default);
}

public sealed class ProductService(
    IAgoraCommerceDbContext dbContext,
    IValidator<CreateProductCommand> createValidator,
    IValidator<UpdateProductCommand> updateValidator) : IProductService
{
    public async Task<Product> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        await createValidator.ValidateAndThrowAsync(command, cancellationToken);

        await EnsureCategoryExists(command.CategoryId, cancellationToken);
        await EnsureUniqueSku(command.Sku, null, cancellationToken);

        var product = Product.Create(
            command.Sku,
            command.Name,
            command.Description,
            command.Price,
            command.Currency ?? "GBP",
            command.CategoryId,
            command.Brand);

        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return product;
    }

    public async Task<Product> UpdateProductAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        await updateValidator.ValidateAndThrowAsync(command, cancellationToken);

        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
        if (product is null)
        {
            throw new NotFoundException($"Product '{command.Id}' was not found.");
        }

        await EnsureCategoryExists(command.CategoryId, cancellationToken);
        await EnsureUniqueSku(command.Sku, command.Id, cancellationToken);

        product.Update(
            command.Sku,
            command.Name,
            command.Description,
            command.Price,
            command.Currency ?? "GBP",
            command.CategoryId,
            command.Brand);

        await dbContext.SaveChangesAsync(cancellationToken);

        return product;
    }

    public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException("Product id is required.");
        }

        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (product is null)
        {
            throw new NotFoundException($"Product '{id}' was not found.");
        }

        product.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);

        return product ?? throw new NotFoundException($"Product '{id}' was not found.");
    }

    public async Task<Product> AdminGetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return product ?? throw new NotFoundException($"Product '{id}' was not found.");
    }

    public async Task<PagedResult<Product>> GetProductsAsync(ProductListQuery query, CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(query);
        var products = dbContext.Products
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ApplyFilters(normalized)
            .ApplySort(normalized.Sort);

        var total = await products.CountAsync(cancellationToken);
        var items = await products
            .Skip((normalized.Page - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(items, normalized.Page, normalized.PageSize, total);
    }

    public async Task<PagedResult<Product>> AdminGetProductsAsync(ProductListQuery query, CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(query);
        var products = dbContext.Products.AsNoTracking().AsQueryable();

        if (normalized.IsActive.HasValue)
        {
            products = products.Where(x => x.IsActive == normalized.IsActive.Value);
        }

        products = products.ApplyFilters(normalized).ApplySort(normalized.Sort);

        var total = await products.CountAsync(cancellationToken);
        var items = await products
            .Skip((normalized.Page - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(items, normalized.Page, normalized.PageSize, total);
    }

    private ProductListQuery Normalize(ProductListQuery query)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        return query with { Page = page, PageSize = pageSize, Sort = string.IsNullOrWhiteSpace(query.Sort) ? "newest" : query.Sort };
    }

    private async Task EnsureCategoryExists(Guid categoryId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Categories.AnyAsync(x => x.Id == categoryId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException($"Category '{categoryId}' was not found.");
        }
    }

    private async Task EnsureUniqueSku(string sku, Guid? productId, CancellationToken cancellationToken)
    {
        var normalizedSku = sku.Trim();
        var exists = await dbContext.Products.AnyAsync(
            x => x.Sku == normalizedSku && (!productId.HasValue || x.Id != productId.Value),
            cancellationToken);

        if (exists)
        {
            throw new ConflictException($"Product SKU '{normalizedSku}' already exists.");
        }
    }
}
