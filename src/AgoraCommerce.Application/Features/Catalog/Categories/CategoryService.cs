using AgoraCommerce.Application.Abstractions;
using AgoraCommerce.Application.Common.Exceptions;
using AgoraCommerce.Application.Common.Models;
using AgoraCommerce.Application.Common.Utilities;
using AgoraCommerce.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AgoraCommerce.Application.Features.Catalog.Categories;

public interface ICategoryService
{
    Task<Category> CreateCategoryAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default);

    Task<Category> UpdateCategoryAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default);

    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Category>> GetPublicCategoriesAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<Category>> GetAdminCategoriesAsync(CategoryListQuery query, CancellationToken cancellationToken = default);
}

public sealed class CategoryService(
    IAgoraCommerceDbContext dbContext,
    IValidator<CreateCategoryCommand> createValidator,
    IValidator<UpdateCategoryCommand> updateValidator) : ICategoryService
{
    public async Task<Category> CreateCategoryAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        await createValidator.ValidateAndThrowAsync(command, cancellationToken);

        var slug = string.IsNullOrWhiteSpace(command.Slug) ? SlugGenerator.Generate(command.Name) : command.Slug.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ValidationException("Slug cannot be empty.");
        }

        var slugExists = await dbContext.Categories.AnyAsync(x => x.Slug == slug, cancellationToken);
        if (slugExists)
        {
            throw new ConflictException($"Category slug '{slug}' already exists.");
        }

        var category = Category.Create(command.Name, slug);
        await dbContext.Categories.AddAsync(category, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        await updateValidator.ValidateAndThrowAsync(command, cancellationToken);

        var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException($"Category '{command.Id}' was not found.");
        }

        var slug = string.IsNullOrWhiteSpace(command.Slug) ? SlugGenerator.Generate(command.Name) : command.Slug.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ValidationException("Slug cannot be empty.");
        }

        var slugExists = await dbContext.Categories.AnyAsync(x => x.Id != command.Id && x.Slug == slug, cancellationToken);
        if (slugExists)
        {
            throw new ConflictException($"Category slug '{slug}' already exists.");
        }

        category.Update(command.Name, slug);
        await dbContext.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException("Category id is required.");
        }

        var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException($"Category '{id}' was not found.");
        }

        category.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetPublicCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Category>> GetAdminCategoriesAsync(CategoryListQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        var categories = dbContext.Categories.AsNoTracking().AsQueryable();
        if (query.IsActive.HasValue)
        {
            categories = categories.Where(x => x.IsActive == query.IsActive.Value);
        }

        var total = await categories.CountAsync(cancellationToken);
        var items = await categories
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Category>(items, page, pageSize, total);
    }
}
