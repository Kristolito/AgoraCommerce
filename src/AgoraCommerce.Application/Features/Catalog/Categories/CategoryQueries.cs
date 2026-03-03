namespace AgoraCommerce.Application.Features.Catalog.Categories;

public sealed record CategoryListQuery(int Page = 1, int PageSize = 20, bool? IsActive = null);
