using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using UsersService.Application.Persistence;
using UsersService.Application.Services;
using UsersService.Infrastructure.Services;
using UsersService.Persistence.Repositories;

namespace AuthService.Tests;

public class AuthServiceFixture : WebApplicationFactory<Program>
{
    public IServiceProvider Services => Server.Services;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            services.AddScoped<IAuthService, UsersService.Infrastructure.Services.AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleRepository, RoleRepository>();
        });

        base.ConfigureWebHost(builder);
    }

    public T GetService<T>() where T : class
    {
        return Services.GetRequiredService<T>();
    }

    public T GetScopedService<T>() where T : class
    {
        using var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }
}
