using FluentValidation;

namespace AgoraCommerce.Application.Features.Basket;

public sealed class GetOrCreateBasketCommandValidator : AbstractValidator<GetOrCreateBasketCommand>
{
    public GetOrCreateBasketCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || x.AnonymousId.HasValue)
            .WithMessage("Either UserId or AnonymousId must be provided.");
    }
}

public sealed class AddBasketItemCommandValidator : AbstractValidator<AddBasketItemCommand>
{
    public AddBasketItemCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || x.AnonymousId.HasValue)
            .WithMessage("Either UserId or AnonymousId must be provided.");

        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}

public sealed class UpdateBasketItemQuantityCommandValidator : AbstractValidator<UpdateBasketItemQuantityCommand>
{
    public UpdateBasketItemQuantityCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || x.AnonymousId.HasValue)
            .WithMessage("Either UserId or AnonymousId must be provided.");

        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}

public sealed class RemoveBasketItemCommandValidator : AbstractValidator<RemoveBasketItemCommand>
{
    public RemoveBasketItemCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || x.AnonymousId.HasValue)
            .WithMessage("Either UserId or AnonymousId must be provided.");

        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}

public sealed class ClearBasketCommandValidator : AbstractValidator<ClearBasketCommand>
{
    public ClearBasketCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || x.AnonymousId.HasValue)
            .WithMessage("Either UserId or AnonymousId must be provided.");
    }
}

public sealed class GetBasketQueryValidator : AbstractValidator<GetBasketQuery>
{
    public GetBasketQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || x.AnonymousId.HasValue)
            .WithMessage("Either UserId or AnonymousId must be provided.");
    }
}
