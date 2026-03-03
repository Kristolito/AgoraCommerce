using AgoraCommerce.Api.Extensions;
using AgoraCommerce.Application.Features.Catalog.Products;
using AgoraCommerce.Contracts.Catalog.Products;
using AgoraCommerce.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace AgoraCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/admin/products")]
public sealed class AdminProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<AdminProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<AdminProductDto>>> GetProducts(
        [FromQuery] GetProductsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new ProductListQuery(
            request.Page,
            request.PageSize,
            request.CategoryId,
            request.Brand,
            request.MinPrice,
            request.MaxPrice,
            request.Search,
            request.Sort,
            request.IsActive);

        var result = await productService.AdminGetProductsAsync(query, cancellationToken);
        return Ok(result.ToPagedResponse(x => x.ToAdminDto()));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AdminProductDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminProductDto>> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var product = await productService.AdminGetProductByIdAsync(id, cancellationToken);
        return Ok(product.ToAdminDto());
    }

    [HttpPost]
    [ProducesResponseType(typeof(AdminProductDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AdminProductDto>> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await productService.CreateProductAsync(
            new CreateProductCommand(
                request.Sku,
                request.Name,
                request.Description,
                request.Price,
                request.Currency,
                request.CategoryId,
                request.Brand),
            cancellationToken);

        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product.ToAdminDto());
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AdminProductDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminProductDto>> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await productService.UpdateProductAsync(
            new UpdateProductCommand(
                id,
                request.Sku,
                request.Name,
                request.Description,
                request.Price,
                request.Currency,
                request.CategoryId,
                request.Brand),
            cancellationToken);

        return Ok(product.ToAdminDto());
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        await productService.DeleteProductAsync(id, cancellationToken);
        return NoContent();
    }
}
