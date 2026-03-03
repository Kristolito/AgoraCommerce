using MySqlConnector;

namespace AgoraCommerce.IntegrationTests.Infrastructure;

public static class MySqlTestGuard
{
    public static async Task<bool> IsAvailableAsync()
    {
        try
        {
            await using var connection = new MySqlConnection("Server=localhost;Port=3306;Database=agoracommerce;User=root;Password=Password123!;");
            await connection.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
