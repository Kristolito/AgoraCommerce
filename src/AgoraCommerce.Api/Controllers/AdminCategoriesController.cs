using AgoraCommerce.Api.Extensions;
using AgoraCommerce.Application.Features.Catalog.Categories;
using AgoraCommerce.Contracts.Catalog.Categories;
using AgoraCommerce.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace AgoraCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/admin/categories")]
public sealed class AdminCategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<AdminCategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<AdminCategoryDto>>> GetCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await categoryService.GetAdminCategoriesAsync(new CategoryListQuery(page, pageSize, isActive), cancellationToken);
        return Ok(result.ToPagedResponse(x => x.ToAdminDto()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(AdminCategoryDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AdminCategoryDto>> CreateCategory(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await categoryService.CreateCategoryAsync(new CreateCategoryCommand(request.Name, request.Slug), cancellationToken);
        return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category.ToAdminDto());
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AdminCategoryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminCategoryDto>> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await categoryService.UpdateCategoryAsync(new UpdateCategoryCommand(id, request.Name, request.Slug), cancellationToken);
        return Ok(category.ToAdminDto());
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        await categoryService.DeleteCategoryAsync(id, cancellationToken);
        return NoContent();
    }
}
