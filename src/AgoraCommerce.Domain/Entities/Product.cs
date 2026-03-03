namespace AgoraCommerce.Domain.Entities;

public class Product
{
    private Product()
    {
    }

    public Guid Id { get; private set; }

    public string Sku { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public string Currency { get; private set; } = "GBP";

    public Guid CategoryId { get; private set; }

    public string? Brand { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public Category Category { get; private set; } = null!;

    public static Product Create(
        string sku,
        string name,
        string? description,
        decimal price,
        string currency,
        Guid categoryId,
        string? brand)
    {
        var now = DateTimeOffset.UtcNow;
        return new Product
        {
            Id = Guid.NewGuid(),
            Sku = EnsureSku(sku),
            Name = EnsureName(name),
            Description = NormalizeOptional(description),
            Price = EnsurePrice(price),
            Currency = EnsureCurrency(currency),
            CategoryId = EnsureCategoryId(categoryId),
            Brand = NormalizeOptional(brand),
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Update(
        string sku,
        string name,
        string? description,
        decimal price,
        string currency,
        Guid categoryId,
        string? brand)
    {
        Sku = EnsureSku(sku);
        Name = EnsureName(name);
        Description = NormalizeOptional(description);
        Price = EnsurePrice(price);
        Currency = EnsureCurrency(currency);
        CategoryId = EnsureCategoryId(categoryId);
        Brand = NormalizeOptional(brand);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Touch(DateTimeOffset updatedAt)
    {
        UpdatedAt = updatedAt;
    }

    private static string EnsureSku(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Product SKU cannot be empty.", nameof(value));
        }

        return value.Trim();
    }

    private static string EnsureName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Product name cannot be empty.", nameof(value));
        }

        return value.Trim();
    }

    private static decimal EnsurePrice(decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Product price must be >= 0.");
        }

        return value;
    }

    private static string EnsureCurrency(string value)
    {
        var normalized = string.IsNullOrWhiteSpace(value) ? "GBP" : value.Trim().ToUpperInvariant();
        if (normalized.Length > 3)
        {
            throw new ArgumentException("Product currency must be at most 3 characters.", nameof(value));
        }

        return normalized;
    }

    private static Guid EnsureCategoryId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("CategoryId is required.", nameof(value));
        }

        return value;
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}
