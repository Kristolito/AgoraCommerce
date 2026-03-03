using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AgoraCommerce.Infrastructure.Persistence;

public class AgoraCommerceDbContextFactory : IDesignTimeDbContextFactory<AgoraCommerceDbContext>
{
    public AgoraCommerceDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Server=localhost;Port=3306;Database=agoracommerce;User=root;Password=Password123!;";
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

        var optionsBuilder = new DbContextOptionsBuilder<AgoraCommerceDbContext>();
        optionsBuilder.UseMySql(connectionString, serverVersion);

        return new AgoraCommerceDbContext(optionsBuilder.Options);
    }
}
