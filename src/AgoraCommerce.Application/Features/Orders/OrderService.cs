using AgoraCommerce.Application.Abstractions;
using AgoraCommerce.Application.Common.Exceptions;
using AgoraCommerce.Application.Common.Models;
using AgoraCommerce.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AgoraCommerce.Application.Features.Orders;

public interface IOrderService
{
    Task<PagedResult<OrderModel>> GetOrdersAsync(GetOrdersQuery query, CancellationToken cancellationToken = default);

    Task<OrderModel> GetOrderByIdAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default);
}

public sealed class OrderService(
    IAgoraCommerceDbContext dbContext,
    IValidator<GetOrdersQuery> getOrdersValidator,
    IValidator<GetOrderByIdQuery> getOrderByIdValidator) : IOrderService
{
    public async Task<PagedResult<OrderModel>> GetOrdersAsync(GetOrdersQuery query, CancellationToken cancellationToken = default)
    {
        await getOrdersValidator.ValidateAndThrowAsync(query, cancellationToken);
        var normalized = query with
        {
            Page = query.Page < 1 ? 1 : query.Page,
            PageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100)
        };

        var ordersQuery = FilterByOwner(normalized.UserId, normalized.AnonymousId);
        var total = await ordersQuery.CountAsync(cancellationToken);
        var orders = await ordersQuery
            .Include(x => x.Lines)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((normalized.Page - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var models = orders.Select(MapOrder).ToList();
        return new PagedResult<OrderModel>(models, normalized.Page, normalized.PageSize, total);
    }

    public async Task<OrderModel> GetOrderByIdAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        await getOrderByIdValidator.ValidateAndThrowAsync(query, cancellationToken);
        var order = await dbContext.Orders
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == query.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Order '{query.OrderId}' was not found.");

        var ownerMatches = query.UserId.HasValue
            ? order.UserId == query.UserId
            : order.AnonymousId == query.AnonymousId;

        if (!ownerMatches)
        {
            throw new ForbiddenException("You do not have access to this order.");
        }

        return MapOrder(order);
    }

    private IQueryable<Order> FilterByOwner(Guid? userId, Guid? anonymousId)
    {
        if (userId.HasValue)
        {
            return dbContext.Orders.Where(x => x.UserId == userId);
        }

        return dbContext.Orders.Where(x => x.AnonymousId == anonymousId);
    }

    private static OrderModel MapOrder(Order order)
    {
        var address = new AddressModel(
            order.ShippingAddress.Line1,
            order.ShippingAddress.Line2,
            order.ShippingAddress.City,
            order.ShippingAddress.Postcode,
            order.ShippingAddress.Country);

        var lines = order.Lines
            .Select(x => new OrderLineModel(x.ProductId, x.ProductName, x.Sku, x.Quantity, x.UnitPrice, x.LineTotal))
            .ToList();

        return new OrderModel(
            order.Id,
            order.OrderNumber,
            order.Status,
            order.Subtotal,
            order.Discount,
            order.Total,
            order.Currency,
            address,
            lines,
            order.CreatedAt);
    }
}
