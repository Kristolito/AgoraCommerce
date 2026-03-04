namespace AgoraCommerce.Application.Features.Basket;

public sealed record GetBasketQuery(Guid? UserId, Guid? AnonymousId);
