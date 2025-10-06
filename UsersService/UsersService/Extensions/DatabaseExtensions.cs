using Microsoft.EntityFrameworkCore;
using UsersService.Application.Persistence.Common;
using UsersService.Persistence.DataContext;

namespace UsersService.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<DataBaseContext>();
                var seeder = services.GetRequiredService<IDataSeeder>();

                // Применяем миграции (опционально)
                await context.Database.MigrateAsync();

                // Заполняем начальными данными
                await seeder.SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }
    }
}
