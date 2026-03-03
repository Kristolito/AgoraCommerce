using AgoraCommerce.Api.Extensions;
using AgoraCommerce.Application.Features.Catalog.Categories;
using AgoraCommerce.Contracts.Catalog.Categories;
using Microsoft.AspNetCore.Mvc;

namespace AgoraCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/categories")]
public sealed class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetPublicCategoriesAsync(cancellationToken);
        return Ok(categories.Select(x => x.ToDto()).ToList());
    }
}
