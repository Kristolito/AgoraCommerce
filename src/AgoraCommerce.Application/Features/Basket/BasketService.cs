using AgoraCommerce.Application.Abstractions;
using AgoraCommerce.Application.Common.Exceptions;
using AgoraCommerce.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using DomainBasket = AgoraCommerce.Domain.Entities.Basket;

namespace AgoraCommerce.Application.Features.Basket;

public interface IBasketService
{
    Task<BasketModel> GetOrCreateBasketAsync(GetOrCreateBasketCommand command, CancellationToken cancellationToken = default);

    Task<BasketModel> GetBasketAsync(GetBasketQuery query, CancellationToken cancellationToken = default);

    Task<BasketModel> AddBasketItemAsync(AddBasketItemCommand command, CancellationToken cancellationToken = default);

    Task<BasketModel> UpdateBasketItemQuantityAsync(UpdateBasketItemQuantityCommand command, CancellationToken cancellationToken = default);

    Task<BasketModel> RemoveBasketItemAsync(RemoveBasketItemCommand command, CancellationToken cancellationToken = default);

    Task<BasketModel> ClearBasketAsync(ClearBasketCommand command, CancellationToken cancellationToken = default);
}

public sealed class BasketService(
    IAgoraCommerceDbContext dbContext,
    IValidator<GetOrCreateBasketCommand> getOrCreateValidator,
    IValidator<GetBasketQuery> getBasketValidator,
    IValidator<AddBasketItemCommand> addItemValidator,
    IValidator<UpdateBasketItemQuantityCommand> updateItemValidator,
    IValidator<RemoveBasketItemCommand> removeItemValidator,
    IValidator<ClearBasketCommand> clearBasketValidator) : IBasketService
{
    public async Task<BasketModel> GetOrCreateBasketAsync(GetOrCreateBasketCommand command, CancellationToken cancellationToken = default)
    {
        await getOrCreateValidator.ValidateAndThrowAsync(command, cancellationToken);
        var basket = await GetOrCreateBasketEntity(command.UserId, command.AnonymousId, cancellationToken);
        return await MapBasketAsync(basket, cancellationToken);
    }

    public async Task<BasketModel> GetBasketAsync(GetBasketQuery query, CancellationToken cancellationToken = default)
    {
        await getBasketValidator.ValidateAndThrowAsync(query, cancellationToken);
        var basket = await GetOrCreateBasketEntity(query.UserId, query.AnonymousId, cancellationToken);
        return await MapBasketAsync(basket, cancellationToken);
    }

    public async Task<BasketModel> AddBasketItemAsync(AddBasketItemCommand command, CancellationToken cancellationToken = default)
    {
        await addItemValidator.ValidateAndThrowAsync(command, cancellationToken);

        var product = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == command.ProductId && x.IsActive, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Active product '{command.ProductId}' was not found.");
        }

        var basket = await GetOrCreateBasketEntity(command.UserId, command.AnonymousId, cancellationToken);
        basket.AddOrIncrementItem(product.Id, command.Quantity, product.Price, product.Currency);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await MapBasketAsync(basket, cancellationToken);
    }

    public async Task<BasketModel> UpdateBasketItemQuantityAsync(UpdateBasketItemQuantityCommand command, CancellationToken cancellationToken = default)
    {
        await updateItemValidator.ValidateAndThrowAsync(command, cancellationToken);
        var basket = await GetExistingBasketEntity(command.UserId, command.AnonymousId, cancellationToken);

        try
        {
            basket.UpdateItemQuantity(command.ProductId, command.Quantity);
        }
        catch (InvalidOperationException ex)
        {
            throw new NotFoundException(ex.Message);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return await MapBasketAsync(basket, cancellationToken);
    }

    public async Task<BasketModel> RemoveBasketItemAsync(RemoveBasketItemCommand command, CancellationToken cancellationToken = default)
    {
        await removeItemValidator.ValidateAndThrowAsync(command, cancellationToken);
        var basket = await GetExistingBasketEntity(command.UserId, command.AnonymousId, cancellationToken);

        try
        {
            basket.RemoveItem(command.ProductId);
        }
        catch (InvalidOperationException ex)
        {
            throw new NotFoundException(ex.Message);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return await MapBasketAsync(basket, cancellationToken);
    }

    public async Task<BasketModel> ClearBasketAsync(ClearBasketCommand command, CancellationToken cancellationToken = default)
    {
        await clearBasketValidator.ValidateAndThrowAsync(command, cancellationToken);
        var basket = await GetOrCreateBasketEntity(command.UserId, command.AnonymousId, cancellationToken);
        basket.Clear();
        await dbContext.SaveChangesAsync(cancellationToken);
        return await MapBasketAsync(basket, cancellationToken);
    }

    private async Task<DomainBasket> GetOrCreateBasketEntity(Guid? userId, Guid? anonymousId, CancellationToken cancellationToken)
    {
        var basket = await QueryBaskets(userId, anonymousId)
            .Include(x => x.Items)
            .FirstOrDefaultAsync(cancellationToken);

        if (basket is not null)
        {
            return basket;
        }

        basket = DomainBasket.Create(userId, anonymousId);
        await dbContext.Baskets.AddAsync(basket, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return basket;
    }

    private async Task<DomainBasket> GetExistingBasketEntity(Guid? userId, Guid? anonymousId, CancellationToken cancellationToken)
    {
        var basket = await QueryBaskets(userId, anonymousId)
            .Include(x => x.Items)
            .FirstOrDefaultAsync(cancellationToken);

        return basket ?? throw new NotFoundException("Basket was not found.");
    }

    private IQueryable<DomainBasket> QueryBaskets(Guid? userId, Guid? anonymousId)
    {
        if (userId.HasValue)
        {
            return dbContext.Baskets.Where(x => x.UserId == userId);
        }

        return dbContext.Baskets.Where(x => x.AnonymousId == anonymousId);
    }

    private async Task<BasketModel> MapBasketAsync(DomainBasket basket, CancellationToken cancellationToken)
    {
        var productIds = basket.Items.Select(x => x.ProductId).Distinct().ToList();
        var productLookup = await dbContext.Products
            .AsNoTracking()
            .Where(x => productIds.Contains(x.Id))
            .Select(x => new { x.Id, x.Name, x.Sku })
            .ToDictionaryAsync(x => x.Id, x => new { x.Name, x.Sku }, cancellationToken);

        var lines = basket.Items
            .Select(item =>
            {
                var product = productLookup.TryGetValue(item.ProductId, out var value) ? value : null;
                var name = product?.Name ?? string.Empty;
                var sku = product?.Sku ?? string.Empty;
                var lineTotal = item.UnitPrice * item.Quantity;
                return new BasketLineModel(item.ProductId, name, sku, item.Quantity, item.UnitPrice, item.Currency, lineTotal);
            })
            .OrderBy(x => x.Name)
            .ToList();

        var subtotal = lines.Sum(x => x.LineTotal);
        return new BasketModel(basket.Id, basket.AnonymousId, lines, subtotal, basket.UpdatedAt);
    }
}
