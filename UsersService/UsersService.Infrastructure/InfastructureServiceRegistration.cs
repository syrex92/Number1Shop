using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;
using UsersService.Application.Services;
using UsersService.Domain.JWT;
using UsersService.Infrastructure.Services;

namespace UsersService.Infrastructure
{
    public static class InfastructureServiceRegistration
    {
        public static IHostBuilder AddLoggerServices(this IHostBuilder hostBuilder, IConfiguration configuration, IHostEnvironment environment)
        {
            // ВЫНЕСЕНА НАСТРОЙКА SERILOG
            ConfigureSerilog(configuration, environment);

            hostBuilder.UseSerilog();

            return hostBuilder;
        }
        public static IServiceCollection AddInfastructureServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ClockSkew = TimeSpan.FromMinutes(5) // Допустимое расхождение времени
                };
                // Для Swagger/тестирования - возможность передавать токен через query string
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        // Поддержка токена в query string для Swagger
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/swagger"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization(options =>
            {
                // Базовая политика
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // Политика для администраторов
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));

                // Политика для модераторов и администраторов
                options.AddPolicy("ModeratorOrAdmin", policy =>
                    policy.RequireRole("Moderator", "Admin"));

                // Кастомная политика с требованием claim
                options.AddPolicy("RequireEmailVerified", policy =>
                    policy.RequireClaim("email_verified", "true"));
            });
            return services;
        }
        // МЕТОД ДЛЯ НАСТРОЙКИ SERILOG
        private static void ConfigureSerilog(IConfiguration configuration, IHostEnvironment environment)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "UsersService")
                .Enrich.WithProperty("Environment", environment.EnvironmentName);

            // Дополнительные настройки в зависимости от среды
            if (environment.IsDevelopment())
            {
                loggerConfiguration
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}",
                        restrictedToMinimumLevel: LogEventLevel.Debug);
            }
            else
            {
                loggerConfiguration
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}");
            }

            // Файловый логгер для всех сред
            loggerConfiguration
                .WriteTo.File(
                    path: "logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 31); // Храним логи за 31 день

            Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}
