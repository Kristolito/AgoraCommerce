using AgoraCommerce.Api.Extensions;
using AgoraCommerce.Application.Features.Coupons;
using AgoraCommerce.Contracts.Coupons;
using Microsoft.AspNetCore.Mvc;

namespace AgoraCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/coupons")]
public sealed class CouponsController(ICouponService couponService) : ControllerBase
{
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidateCouponResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ValidateCouponResponse>> ValidateCoupon(
        [FromBody] ValidateCouponRequest request,
        CancellationToken cancellationToken)
    {
        var result = await couponService.ValidateCouponAsync(
            new ValidateCouponQuery(request.Code, request.Subtotal),
            cancellationToken);

        return Ok(result.ToDto());
    }
}
