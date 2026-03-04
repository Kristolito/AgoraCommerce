namespace AgoraCommerce.Application.Features.Basket;

public sealed record GetOrCreateBasketCommand(Guid? UserId, Guid? AnonymousId);

public sealed record AddBasketItemCommand(Guid? UserId, Guid? AnonymousId, Guid ProductId, int Quantity);

public sealed record UpdateBasketItemQuantityCommand(Guid? UserId, Guid? AnonymousId, Guid ProductId, int Quantity);

public sealed record RemoveBasketItemCommand(Guid? UserId, Guid? AnonymousId, Guid ProductId);

public sealed record ClearBasketCommand(Guid? UserId, Guid? AnonymousId);
