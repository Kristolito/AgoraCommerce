namespace AgoraCommerce.Domain.Entities;

public class OrderLine
{
    private OrderLine()
    {
    }

    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public string Sku { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal LineTotal { get; private set; }

    public Order Order { get; private set; } = null!;

    public static OrderLine Create(Guid productId, string productName, string sku, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be > 0.");
        }

        return new OrderLine
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ProductName = productName.Trim(),
            Sku = sku.Trim(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            LineTotal = unitPrice * quantity
        };
    }

    public void AttachToOrder(Guid orderId)
    {
        OrderId = orderId;
    }
}
