using LeaveFlowHR.Api.Infrastructure.Database;

namespace LeaveFlowHR.Api.Infrastructure.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context =
            scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await DbSeeder.SeedAsync(context);
    }
}