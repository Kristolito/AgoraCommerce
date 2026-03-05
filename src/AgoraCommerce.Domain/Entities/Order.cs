using AgoraCommerce.Domain.Enums;
using AgoraCommerce.Domain.ValueObjects;

namespace AgoraCommerce.Domain.Entities;

public class Order
{
    private Order()
    {
    }

    public Guid Id { get; private set; }

    public string OrderNumber { get; private set; } = string.Empty;

    public Guid? UserId { get; private set; }

    public Guid? AnonymousId { get; private set; }

    public OrderStatus Status { get; private set; }

    public decimal Subtotal { get; private set; }

    public decimal Discount { get; private set; }

    public decimal Total { get; private set; }

    public string Currency { get; private set; } = "GBP";

    public Address ShippingAddress { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public List<OrderLine> Lines { get; private set; } = new();

    public static Order Create(
        string orderNumber,
        Guid? userId,
        Guid? anonymousId,
        string currency,
        Address shippingAddress,
        IEnumerable<OrderLine> lines,
        decimal discount)
    {
        if (!userId.HasValue && !anonymousId.HasValue)
        {
            throw new ArgumentException("Order owner is required.");
        }

        var list = lines.ToList();
        if (list.Count == 0)
        {
            throw new ArgumentException("Order requires at least one line.");
        }

        var orderId = Guid.NewGuid();
        foreach (var line in list)
        {
            line.AttachToOrder(orderId);
        }

        var subtotal = list.Sum(x => x.LineTotal);
        var total = subtotal - discount;
        if (total < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(discount), "Discount cannot exceed subtotal.");
        }

        return new Order
        {
            Id = orderId,
            OrderNumber = orderNumber.Trim(),
            UserId = userId,
            AnonymousId = anonymousId,
            Status = OrderStatus.PendingPayment,
            Subtotal = subtotal,
            Discount = discount,
            Total = total,
            Currency = string.IsNullOrWhiteSpace(currency) ? "GBP" : currency.Trim().ToUpperInvariant(),
            ShippingAddress = shippingAddress,
            CreatedAt = DateTimeOffset.UtcNow,
            Lines = list
        };
    }
}
