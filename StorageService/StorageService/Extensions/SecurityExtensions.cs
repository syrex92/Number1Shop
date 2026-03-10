using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace StorageService.Extensions;

internal static class SecurityExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Получаем настройки Keycloak из конфигурации
        var authServerUrl = configuration["Keycloak:AuthServerUrl"] ?? "http://keycloak:8080";
        var realm = configuration["Keycloak:Realm"] ?? "shop";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Адрес Keycloak
                options.Authority = $"{authServerUrl}/realms/{realm}";
                options.MetadataAddress = $"{authServerUrl}/realms/{realm}/.well-known/openid-configuration";
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                // Параметры проверки токена
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"{authServerUrl}/realms/{realm}",
                    ValidateAudience = false, // Для простоты
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2),
                    NameClaimType = "preferred_username",
                    RoleClaimType = "realm_access"
                };

                // Логирование для отладки
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Log.Error(context.Exception, "Authentication failed: {Message}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Log.Warning("Challenge: {Error}, {ErrorDescription}", context.Error, context.ErrorDescription);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var userId = context.Principal?.FindFirst("sub")?.Value;
                        var userName = context.Principal?.FindFirst("preferred_username")?.Value;
                        Log.Information("User {UserName} ({UserId}) authenticated successfully", userName, userId);
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}