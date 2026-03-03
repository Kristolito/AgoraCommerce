namespace AgoraCommerce.Application.Features.Catalog.Categories;

public sealed record CreateCategoryCommand(string Name, string? Slug);

public sealed record UpdateCategoryCommand(Guid Id, string Name, string? Slug);
