using AgoraCommerce.Domain.Enums;
using FluentValidation;

namespace AgoraCommerce.Application.Features.Coupons;

public sealed class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.MaxRedemptions).GreaterThan(0).When(x => x.MaxRedemptions.HasValue);
        RuleFor(x => x.Currency).MaximumLength(3);
        RuleFor(x => x).Must(x => x.Type != CouponType.Percent || x.Amount <= 100)
            .WithMessage("Percent coupon amount must be <= 100.");
    }
}

public sealed class UpdateCouponCommandValidator : AbstractValidator<UpdateCouponCommand>
{
    public UpdateCouponCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.MaxRedemptions).GreaterThan(0).When(x => x.MaxRedemptions.HasValue);
        RuleFor(x => x.Currency).MaximumLength(3);
        RuleFor(x => x).Must(x => x.Type != CouponType.Percent || x.Amount <= 100)
            .WithMessage("Percent coupon amount must be <= 100.");
    }
}

public sealed class DeactivateCouponCommandValidator : AbstractValidator<DeactivateCouponCommand>
{
    public DeactivateCouponCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class AdminGetCouponByIdQueryValidator : AbstractValidator<AdminGetCouponByIdQuery>
{
    public AdminGetCouponByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class AdminGetCouponsQueryValidator : AbstractValidator<AdminGetCouponsQuery>
{
    public AdminGetCouponsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class ValidateCouponQueryValidator : AbstractValidator<ValidateCouponQuery>
{
    public ValidateCouponQueryValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Subtotal).GreaterThanOrEqualTo(0);
    }
}
