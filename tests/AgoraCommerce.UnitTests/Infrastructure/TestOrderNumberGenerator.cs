using AgoraCommerce.Application.Abstractions;

namespace AgoraCommerce.UnitTests.Infrastructure;

public sealed class TestOrderNumberGenerator : IOrderNumberGenerator
{
    private int _counter;

    public string Generate(DateTimeOffset now)
    {
        _counter++;
        return $"AG-{now:yyyyMMdd}-{_counter:D6}";
    }
}
