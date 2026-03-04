namespace AgoraCommerce.Domain.Entities;

public class BasketItem
{
    private BasketItem()
    {
    }

    public Guid Id { get; private set; }

    public Guid BasketId { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public string Currency { get; private set; } = "GBP";

    public Basket Basket { get; private set; } = null!;

    public static BasketItem Create(Guid basketId, Guid productId, int quantity, decimal unitPrice, string currency)
    {
        EnsureQuantity(quantity);

        return new BasketItem
        {
            Id = Guid.NewGuid(),
            BasketId = basketId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Currency = string.IsNullOrWhiteSpace(currency) ? "GBP" : currency.Trim().ToUpperInvariant()
        };
    }

    public void SetQuantity(int quantity)
    {
        EnsureQuantity(quantity);
        Quantity = quantity;
    }

    public void IncrementQuantity(int amount)
    {
        EnsureQuantity(amount);
        Quantity += amount;
    }

    private static void EnsureQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }
    }
}
