using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UsersService.Application.Persistence;
using UsersService.Application.Persistence.Common;
using UsersService.Persistence.DataContext;
using UsersService.Persistence.Repositories;
using UsersService.Persistence.Repositories.Common;

namespace UsersService.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistanceServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddDbContext<DataBaseContext>(options =>
            {
                if (environment.IsDevelopment() || environment.IsEnvironment("Test"))
                {
                    options.UseSqlite("Data Source=auth-service-db.db");
                }
                else
                {
                    options.UseNpgsql(configuration.GetConnectionString("AppDBcontext"));
                }
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            // Регистрация сидера
            services.AddScoped<IDataSeeder, DataSeeder>();
            return services;
        }
    }
}
