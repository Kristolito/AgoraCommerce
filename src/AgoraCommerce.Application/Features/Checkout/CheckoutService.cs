using AgoraCommerce.Application.Abstractions;
using AgoraCommerce.Application.Common.Exceptions;
using AgoraCommerce.Domain.Entities;
using AgoraCommerce.Domain.Enums;
using AgoraCommerce.Domain.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using DomainBasket = AgoraCommerce.Domain.Entities.Basket;

namespace AgoraCommerce.Application.Features.Checkout;

public interface ICheckoutService
{
    Task<CheckoutResultModel> CheckoutBasketAsync(CheckoutBasketCommand command, CancellationToken cancellationToken = default);
}

public sealed class CheckoutService(
    IAgoraCommerceDbContext dbContext,
    IOrderNumberGenerator orderNumberGenerator,
    IValidator<CheckoutBasketCommand> validator) : ICheckoutService
{
    public async Task<CheckoutResultModel> CheckoutBasketAsync(CheckoutBasketCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var ownerType = command.UserId.HasValue ? CheckoutOwnerType.User : CheckoutOwnerType.Anonymous;

        await using var transaction = await dbContext.BeginTransactionAsync(cancellationToken);

        var existingRequest = await dbContext.CheckoutRequests
            .FirstOrDefaultAsync(x =>
                x.OwnerType == ownerType &&
                x.UserId == command.UserId &&
                x.AnonymousId == command.AnonymousId &&
                x.IdempotencyKey == command.IdempotencyKey,
                cancellationToken);

        if (existingRequest is { Status: CheckoutRequestStatus.Processed } && existingRequest.OrderId.HasValue)
        {
            var existingOrder = await dbContext.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == existingRequest.OrderId.Value, cancellationToken)
                ?? throw new NotFoundException("Existing order for idempotency key was not found.");

            await transaction.CommitAsync(cancellationToken);
            return ToCheckoutResult(existingOrder, true);
        }

        if (existingRequest is not null)
        {
            throw new ConflictException("Checkout idempotency key is already in use.");
        }

        var idempotency = CheckoutRequest.Create(ownerType, command.AnonymousId, command.UserId, command.IdempotencyKey);
        await dbContext.CheckoutRequests.AddAsync(idempotency, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            var basket = await LoadBasket(command.UserId, command.AnonymousId, cancellationToken);
            if (basket.Items.Count == 0)
            {
                throw new ConflictException("Cannot checkout an empty basket.");
            }

            var productLookup = await dbContext.Products
                .AsNoTracking()
                .Where(x => basket.Items.Select(i => i.ProductId).Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var lines = basket.Items.Select(item =>
            {
                if (!productLookup.TryGetValue(item.ProductId, out var product))
                {
                    throw new NotFoundException($"Product '{item.ProductId}' was not found for checkout.");
                }

                return OrderLine.Create(item.ProductId, product.Name, product.Sku, item.Quantity, item.UnitPrice);
            }).ToList();

            var address = Address.Create(
                command.ShippingAddress.Line1,
                command.ShippingAddress.Line2,
                command.ShippingAddress.City,
                command.ShippingAddress.Postcode,
                command.ShippingAddress.Country);

            var now = DateTimeOffset.UtcNow;
            var orderNumber = orderNumberGenerator.Generate(now);
            var currency = basket.Items.Select(x => x.Currency).FirstOrDefault() ?? "GBP";
            var order = Order.Create(orderNumber, command.UserId, command.AnonymousId, currency, address, lines, 0);

            await dbContext.Orders.AddAsync(order, cancellationToken);
            basket.Clear();
            idempotency.MarkProcessed(order.Id);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return ToCheckoutResult(order, false);
        }
        catch (Exception ex)
        {
            idempotency.MarkFailed(ex.Message);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<DomainBasket> LoadBasket(Guid? userId, Guid? anonymousId, CancellationToken cancellationToken)
    {
        var query = dbContext.Baskets.Include(x => x.Items).AsQueryable();
        if (userId.HasValue)
        {
            query = query.Where(x => x.UserId == userId);
        }
        else
        {
            query = query.Where(x => x.AnonymousId == anonymousId);
        }

        return await query.FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Basket was not found.");
    }

    private static CheckoutResultModel ToCheckoutResult(Order order, bool isReplay) =>
        new(
            order.Id,
            order.OrderNumber,
            order.Status,
            order.Subtotal,
            order.Discount,
            order.Total,
            order.Currency,
            order.CreatedAt,
            isReplay);
}
