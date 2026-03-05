namespace AgoraCommerce.Application.Abstractions;

public interface IOrderNumberGenerator
{
    string Generate(DateTimeOffset now);
}
