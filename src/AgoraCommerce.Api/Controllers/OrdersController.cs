using System.Security.Claims;
using AgoraCommerce.Api.Extensions;
using AgoraCommerce.Application.Features.Orders;
using AgoraCommerce.Contracts.Common;
using AgoraCommerce.Contracts.Orders;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AgoraCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/orders")]
public sealed class OrdersController(IOrderService orderService) : ControllerBase
{
    private const string AnonymousHeader = "X-Anonymous-Id";

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<OrderDto>>> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (userId, anonymousId) = ResolveIdentity();
        var result = await orderService.GetOrdersAsync(new GetOrdersQuery(userId, anonymousId, page, pageSize), cancellationToken);
        EnsureAnonymousHeader(anonymousId);
        return Ok(result.ToPagedResponse(x => x.ToDto()));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var (userId, anonymousId) = ResolveIdentity();
        var order = await orderService.GetOrderByIdAsync(new GetOrderByIdQuery(userId, anonymousId, id), cancellationToken);
        EnsureAnonymousHeader(anonymousId);
        return Ok(order.ToDto());
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

        return (null, Guid.NewGuid());
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
