using AgoraCommerce.Application.Abstractions;
using AgoraCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgoraCommerce.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' was not found.");

        ServerVersion serverVersion;
        try
        {
            serverVersion = ServerVersion.AutoDetect(connectionString);
        }
        catch
        {
            // Allows design-time tooling to run when MySQL is not reachable.
            serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
        }

        services.AddDbContext<AgoraCommerceDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));
        services.AddScoped<IAgoraCommerceDbContext>(sp => sp.GetRequiredService<AgoraCommerceDbContext>());

        return services;
    }
}
