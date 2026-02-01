using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace StorageService.Extensions
{
    internal static class SwaggerExtensions
    {
        public static IServiceCollection AddSecuredSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("", new OpenApiInfo { Title = "Shop.StorageService.Extensions", Version = "v1" });
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Storage API", Version = "v1", Description = "Shop.Storage Service API" });

                var basePath = AppContext.BaseDirectory;

                var xmlPath = Path.Combine(basePath, "Shop.StorageService.xml");
                c.IncludeXmlComments(xmlPath);

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // must be lower case
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });
            });

            return services;
        }
    }
}
