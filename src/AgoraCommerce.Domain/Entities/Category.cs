namespace AgoraCommerce.Domain.Entities;

public class Category
{
    private Category()
    {
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public ICollection<Product> Products { get; private set; } = new List<Product>();

    public static Category Create(string name, string slug)
    {
        var now = DateTimeOffset.UtcNow;
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = EnsureName(name),
            Slug = EnsureSlug(slug),
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Update(string name, string slug)
    {
        Name = EnsureName(name);
        Slug = EnsureSlug(slug);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Touch(DateTimeOffset updatedAt)
    {
        UpdatedAt = updatedAt;
    }

    private static string EnsureName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Category name cannot be empty.", nameof(value));
        }

        return value.Trim();
    }

    private static string EnsureSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Category slug cannot be empty.", nameof(value));
        }

        return value.Trim();
    }
}
