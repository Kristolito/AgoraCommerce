using System.Security.Claims;
using AgoraCommerce.Api.Extensions;
using AgoraCommerce.Application.Features.Checkout;
using AgoraCommerce.Contracts.Checkout;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AgoraCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/checkout")]
public sealed class CheckoutController(ICheckoutService checkoutService) : ControllerBase
{
    private const string AnonymousHeader = "X-Anonymous-Id";
    private const string IdempotencyHeader = "Idempotency-Key";

    [HttpPost]
    [ProducesResponseType(typeof(CheckoutResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CheckoutResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CheckoutResponse>> Checkout(
        [FromBody] CheckoutRequest request,
        CancellationToken cancellationToken)
    {
        if (!Request.Headers.TryGetValue(IdempotencyHeader, out var idempotencyValues) || string.IsNullOrWhiteSpace(idempotencyValues.FirstOrDefault()))
        {
            throw new ValidationException($"{IdempotencyHeader} header is required.");
        }

        var (userId, anonymousId) = ResolveIdentity();
        var command = new CheckoutBasketCommand(
            userId,
            anonymousId,
            idempotencyValues.First()!,
            new CheckoutAddressModel(
                request.ShippingAddress.Line1,
                request.ShippingAddress.Line2,
                request.ShippingAddress.City,
                request.ShippingAddress.Postcode,
                request.ShippingAddress.Country));

        var result = await checkoutService.CheckoutBasketAsync(command, cancellationToken);
        EnsureAnonymousHeader(anonymousId);
        var dto = result.ToDto();

        if (result.IsFromIdempotencyReplay)
        {
            return Ok(dto);
        }

        return Created($"/api/v1/orders/{result.OrderId}", dto);
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
