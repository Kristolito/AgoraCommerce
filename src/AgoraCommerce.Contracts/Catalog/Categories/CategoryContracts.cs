namespace AgoraCommerce.Contracts.Catalog.Categories;

public sealed record CategoryDto(Guid Id, string Name, string Slug);

public sealed record AdminCategoryDto(Guid Id, string Name, string Slug, bool IsActive, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

public sealed record CreateCategoryRequest(string Name, string? Slug);

public sealed record UpdateCategoryRequest(string Name, string? Slug);
