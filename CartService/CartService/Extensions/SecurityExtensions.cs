using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Shop.CartService.Extensions;

internal static class SecurityExtensions
{
    private static ClaimsPrincipal? GetPrincipal(string token)
    {
        const string secret = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ==";
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            if (jwtToken == null)
                return null;
            byte[] key = Convert.FromBase64String(secret);
            var parameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            var principal = tokenHandler.ValidateToken(token,
                parameters, out _);
            return principal;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
    
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = BearerTokenDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters.LogValidationExceptions = true;
                options.TokenValidationParameters.ValidateAudience = false;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidateIssuer = false;
                options.SaveToken = true;
                options.TokenValidationParameters.RoleClaimType = "roles";
                options.TokenValidationParameters.NameClaimType = "preferred_username";
                options.TokenValidationParameters.SaveSigninToken = true;

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (!context.Request.Headers.TryGetValue("Authorization", out var value))
                            return Task.CompletedTask;
                        try
                        {
                            var basicToken = value.ToString()
                                .Trim()
                                .Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                .LastOrDefault() ?? string.Empty;
                            context.Principal = GetPrincipal(basicToken);
                            context.Success();

                            return Task.CompletedTask;
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "Failed to parse JWT token: [{msg}]", e.Message);
                            context.Fail(e);
                            return Task.CompletedTask;
                        }
                    }
                };
            });

        return services;

    }
}