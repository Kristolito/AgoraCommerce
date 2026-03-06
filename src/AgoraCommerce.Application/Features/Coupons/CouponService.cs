using AgoraCommerce.Application.Abstractions;
using AgoraCommerce.Application.Common.Exceptions;
using AgoraCommerce.Application.Common.Models;
using AgoraCommerce.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AgoraCommerce.Application.Features.Coupons;

public interface ICouponService
{
    Task<CouponModel> CreateCouponAsync(CreateCouponCommand command, CancellationToken cancellationToken = default);

    Task<CouponModel> UpdateCouponAsync(UpdateCouponCommand command, CancellationToken cancellationToken = default);

    Task DeactivateCouponAsync(DeactivateCouponCommand command, CancellationToken cancellationToken = default);

    Task<PagedResult<CouponModel>> AdminGetCouponsAsync(AdminGetCouponsQuery query, CancellationToken cancellationToken = default);

    Task<CouponModel> AdminGetCouponByIdAsync(AdminGetCouponByIdQuery query, CancellationToken cancellationToken = default);

    Task<CouponValidationModel> ValidateCouponAsync(ValidateCouponQuery query, CancellationToken cancellationToken = default);
}

public sealed class CouponService(
    IAgoraCommerceDbContext dbContext,
    IValidator<CreateCouponCommand> createValidator,
    IValidator<UpdateCouponCommand> updateValidator,
    IValidator<DeactivateCouponCommand> deactivateValidator,
    IValidator<AdminGetCouponsQuery> adminListValidator,
    IValidator<AdminGetCouponByIdQuery> adminByIdValidator,
    IValidator<ValidateCouponQuery> validateValidator) : ICouponService
{
    public async Task<CouponModel> CreateCouponAsync(CreateCouponCommand command, CancellationToken cancellationToken = default)
    {
        await createValidator.ValidateAndThrowAsync(command, cancellationToken);
        var code = command.Code.Trim().ToUpperInvariant();

        var exists = await dbContext.Coupons.AnyAsync(x => x.Code == code, cancellationToken);
        if (exists)
        {
            throw new ConflictException($"Coupon code '{code}' already exists.");
        }

        var coupon = Coupon.Create(
            code,
            command.Type,
            command.Amount,
            command.Currency,
            command.IsActive,
            command.ActiveFrom,
            command.ActiveTo,
            command.MaxRedemptions);

        await dbContext.Coupons.AddAsync(coupon, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(coupon);
    }

    public async Task<CouponModel> UpdateCouponAsync(UpdateCouponCommand command, CancellationToken cancellationToken = default)
    {
        await updateValidator.ValidateAndThrowAsync(command, cancellationToken);

        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException($"Coupon '{command.Id}' was not found.");

        coupon.Update(
            command.Type,
            command.Amount,
            command.Currency,
            command.IsActive,
            command.ActiveFrom,
            command.ActiveTo,
            command.MaxRedemptions);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(coupon);
    }

    public async Task DeactivateCouponAsync(DeactivateCouponCommand command, CancellationToken cancellationToken = default)
    {
        await deactivateValidator.ValidateAndThrowAsync(command, cancellationToken);
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException($"Coupon '{command.Id}' was not found.");

        coupon.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<CouponModel>> AdminGetCouponsAsync(AdminGetCouponsQuery query, CancellationToken cancellationToken = default)
    {
        await adminListValidator.ValidateAndThrowAsync(query, cancellationToken);
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        var coupons = dbContext.Coupons.AsNoTracking().OrderByDescending(x => x.CreatedAt);
        var total = await coupons.CountAsync(cancellationToken);
        var items = await coupons.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<CouponModel>(items.Select(Map).ToList(), page, pageSize, total);
    }

    public async Task<CouponModel> AdminGetCouponByIdAsync(AdminGetCouponByIdQuery query, CancellationToken cancellationToken = default)
    {
        await adminByIdValidator.ValidateAndThrowAsync(query, cancellationToken);
        var coupon = await dbContext.Coupons.AsNoTracking().FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken)
            ?? throw new NotFoundException($"Coupon '{query.Id}' was not found.");
        return Map(coupon);
    }

    public async Task<CouponValidationModel> ValidateCouponAsync(ValidateCouponQuery query, CancellationToken cancellationToken = default)
    {
        await validateValidator.ValidateAndThrowAsync(query, cancellationToken);
        var coupon = await dbContext.Coupons.AsNoTracking().FirstOrDefaultAsync(
            x => x.Code == query.Code.Trim().ToUpperInvariant(),
            cancellationToken);

        if (coupon is null)
        {
            return new CouponValidationModel(false, "Coupon not found.", 0, query.Subtotal);
        }

        var now = DateTimeOffset.UtcNow;
        if (!coupon.IsValidAt(now))
        {
            return new CouponValidationModel(false, "Coupon is inactive, expired, or max redemptions reached.", 0, query.Subtotal);
        }

        var discount = coupon.CalculateDiscount(query.Subtotal);
        var totalAfter = Math.Max(0, query.Subtotal - discount);
        return new CouponValidationModel(true, null, discount, totalAfter);
    }

    private static CouponModel Map(Coupon coupon)
    {
        return new CouponModel(
            coupon.Id,
            coupon.Code,
            coupon.Type,
            coupon.Amount,
            coupon.Currency,
            coupon.IsActive,
            coupon.ActiveFrom,
            coupon.ActiveTo,
            coupon.MaxRedemptions,
            coupon.RedeemedCount,
            coupon.CreatedAt,
            coupon.UpdatedAt);
    }
}
