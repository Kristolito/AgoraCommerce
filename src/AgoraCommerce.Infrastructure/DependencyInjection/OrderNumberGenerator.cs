using AgoraCommerce.Application.Abstractions;

namespace AgoraCommerce.Infrastructure.DependencyInjection;

public sealed class OrderNumberGenerator : IOrderNumberGenerator
{
    public string Generate(DateTimeOffset now)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        return $"AG-{now:yyyyMMdd}-{suffix}";
    }
}
