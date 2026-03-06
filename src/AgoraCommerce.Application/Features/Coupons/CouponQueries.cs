namespace AgoraCommerce.Application.Features.Coupons;

public sealed record AdminGetCouponsQuery(int Page = 1, int PageSize = 20);

public sealed record AdminGetCouponByIdQuery(Guid Id);

public sealed record ValidateCouponQuery(string Code, decimal Subtotal);
