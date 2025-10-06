using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Persistence;
using UsersService.Application.Persistence.Common;
using UsersService.Persistence.DataContext;
using UsersService.Persistence.Repositories;
using UsersService.Persistence.Repositories.Common;

namespace UsersService.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistanceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataBaseContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("AppDBcontext")));

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
