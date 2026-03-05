namespace AgoraCommerce.Domain.ValueObjects;

public class Address
{
    private Address()
    {
    }

    public string Line1 { get; private set; } = string.Empty;

    public string? Line2 { get; private set; }

    public string City { get; private set; } = string.Empty;

    public string Postcode { get; private set; } = string.Empty;

    public string Country { get; private set; } = string.Empty;

    public static Address Create(string line1, string? line2, string city, string postcode, string country)
    {
        return new Address
        {
            Line1 = EnsureRequired(line1, nameof(line1)),
            Line2 = NormalizeOptional(line2),
            City = EnsureRequired(city, nameof(city)),
            Postcode = EnsureRequired(postcode, nameof(postcode)),
            Country = EnsureRequired(country, nameof(country)).ToUpperInvariant()
        };
    }

    private static string EnsureRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Address field is required.", paramName);
        }

        return value.Trim();
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
