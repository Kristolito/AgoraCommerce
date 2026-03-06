using FluentValidation;

namespace AgoraCommerce.Application.Features.Checkout;

public sealed class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || x.AnonymousId.HasValue)
            .WithMessage("Either UserId or AnonymousId must be provided.");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CouponCode)
            .MaximumLength(64);

        RuleFor(x => x.ShippingAddress.Line1)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ShippingAddress.Line2)
            .MaximumLength(200);

        RuleFor(x => x.ShippingAddress.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ShippingAddress.Postcode)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.ShippingAddress.Country)
            .NotEmpty()
            .MaximumLength(2);
    }
}
