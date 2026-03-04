using System.Security.Claims;
using AgoraCommerce.Api.Extensions;
using AgoraCommerce.Application.Features.Basket;
using AgoraCommerce.Contracts.Basket;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AgoraCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/basket")]
public sealed class BasketController(IBasketService basketService) : ControllerBase
{
    private const string AnonymousHeader = "X-Anonymous-Id";

    [HttpGet]
    [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BasketDto>> GetBasket(CancellationToken cancellationToken)
    {
        var (userId, anonymousId) = ResolveIdentity();
        var basket = await basketService.GetBasketAsync(new GetBasketQuery(userId, anonymousId), cancellationToken);
        EnsureAnonymousHeader(anonymousId);
        return Ok(basket.ToDto());
    }

    [HttpPost("items")]
    [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BasketDto>> AddBasketItem(
        [FromBody] AddBasketItemRequest request,
        CancellationToken cancellationToken)
    {
        var (userId, anonymousId) = ResolveIdentity();
        var basket = await basketService.AddBasketItemAsync(
            new AddBasketItemCommand(userId, anonymousId, request.ProductId, request.Quantity),
            cancellationToken);

        EnsureAnonymousHeader(anonymousId);
        return Ok(basket.ToDto());
    }

    [HttpPut("items/{productId:guid}")]
    [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BasketDto>> UpdateBasketItemQuantity(
        Guid productId,
        [FromBody] UpdateBasketItemQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var (userId, anonymousId) = ResolveIdentity();
        var basket = await basketService.UpdateBasketItemQuantityAsync(
            new UpdateBasketItemQuantityCommand(userId, anonymousId, productId, request.Quantity),
            cancellationToken);

        EnsureAnonymousHeader(anonymousId);
        return Ok(basket.ToDto());
    }

    [HttpDelete("items/{productId:guid}")]
    [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BasketDto>> RemoveBasketItem(Guid productId, CancellationToken cancellationToken)
    {
        var (userId, anonymousId) = ResolveIdentity();
        var basket = await basketService.RemoveBasketItemAsync(
            new RemoveBasketItemCommand(userId, anonymousId, productId),
            cancellationToken);

        EnsureAnonymousHeader(anonymousId);
        return Ok(basket.ToDto());
    }

    [HttpDelete]
    [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BasketDto>> ClearBasket(CancellationToken cancellationToken)
    {
        var (userId, anonymousId) = ResolveIdentity();
        var basket = await basketService.ClearBasketAsync(new ClearBasketCommand(userId, anonymousId), cancellationToken);
        EnsureAnonymousHeader(anonymousId);
        return Ok(basket.ToDto());
    }

    private (Guid? UserId, Guid? AnonymousId) ResolveIdentity()
    {
        var userId = TryGetUserId();
        if (userId.HasValue)
        {
            return (userId, null);
        }

        if (Request.Headers.TryGetValue(AnonymousHeader, out var values) && values.Count > 0)
        {
            if (Guid.TryParse(values[0], out var parsedAnonymousId))
            {
                return (null, parsedAnonymousId);
            }

            throw new ValidationException($"{AnonymousHeader} header must be a valid GUID.");
        }

        var generated = Guid.NewGuid();
        return (null, generated);
    }

    private Guid? TryGetUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(raw, out var parsed) ? parsed : null;
    }

    private void EnsureAnonymousHeader(Guid? anonymousId)
    {
        if (anonymousId.HasValue)
        {
            Response.Headers[AnonymousHeader] = anonymousId.Value.ToString();
        }
    }
}
