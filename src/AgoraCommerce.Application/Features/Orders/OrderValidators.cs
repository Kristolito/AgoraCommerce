using FluentValidation;

namespace AgoraCommerce.Application.Features.Orders;

public sealed class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
{
    public GetOrdersQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || x.AnonymousId.HasValue)
            .WithMessage("Either UserId or AnonymousId must be provided.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}

public sealed class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || x.AnonymousId.HasValue)
            .WithMessage("Either UserId or AnonymousId must be provided.");

        RuleFor(x => x.OrderId)
            .NotEmpty();
    }
}
