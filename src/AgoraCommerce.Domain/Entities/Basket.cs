namespace AgoraCommerce.Domain.Entities;

public class Basket
{
    private Basket()
    {
    }

    public Guid Id { get; private set; }

    public Guid? UserId { get; private set; }

    public Guid? AnonymousId { get; private set; }

    public List<BasketItem> Items { get; private set; } = new();

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Basket Create(Guid? userId, Guid? anonymousId)
    {
        if (!userId.HasValue && !anonymousId.HasValue)
        {
            throw new ArgumentException("Either userId or anonymousId must be provided.");
        }

        return new Basket
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AnonymousId = anonymousId,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public void AddOrIncrementItem(Guid productId, int quantity, decimal unitPrice, string currency)
    {
        EnsureQuantity(quantity);

        var existing = Items.FirstOrDefault(x => x.ProductId == productId);
        if (existing is not null)
        {
            existing.IncrementQuantity(quantity);
            Touch();
            return;
        }

        Items.Add(BasketItem.Create(Id, productId, quantity, unitPrice, currency));
        Touch();
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        EnsureQuantity(quantity);
        var item = Items.FirstOrDefault(x => x.ProductId == productId)
            ?? throw new InvalidOperationException("Basket item not found.");

        item.SetQuantity(quantity);
        Touch();
    }

    public void RemoveItem(Guid productId)
    {
        var removed = Items.RemoveAll(x => x.ProductId == productId);
        if (removed == 0)
        {
            throw new InvalidOperationException("Basket item not found.");
        }

        Touch();
    }

    public void Clear()
    {
        Items.Clear();
        Touch();
    }

    public decimal GetSubtotal() => Items.Sum(x => x.UnitPrice * x.Quantity);

    public void Touch()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static void EnsureQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }
    }
}
