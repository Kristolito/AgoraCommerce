using AgoraCommerce.Api.Extensions;
using AgoraCommerce.Application.Features.Catalog.Products;
using AgoraCommerce.Contracts.Catalog.Products;
using AgoraCommerce.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace AgoraCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetProducts(
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
            null);

        var result = await productService.GetProductsAsync(query, cancellationToken);
        return Ok(result.ToPagedResponse(x => x.ToDto()));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var product = await productService.GetProductByIdAsync(id, cancellationToken);
        return Ok(product.ToDto());
    }
}
