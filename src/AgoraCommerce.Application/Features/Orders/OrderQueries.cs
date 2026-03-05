namespace AgoraCommerce.Application.Features.Orders;

public sealed record GetOrdersQuery(Guid? UserId, Guid? AnonymousId, int Page = 1, int PageSize = 20);

public sealed record GetOrderByIdQuery(Guid? UserId, Guid? AnonymousId, Guid OrderId);
