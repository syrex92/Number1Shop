using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth Service", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint
app.MapHealthChecks("/health");

// Public endpoint
app.MapGet("/public", () => "Public endpoint - no auth required");

// Protected endpoint
app.MapGet("/protected", () => "Protected endpoint - auth required")
    .RequireAuthorization();

// Admin endpoint
app.MapGet("/admin", () => "Admin endpoint")
    .RequireAuthorization(policy => policy.RequireRole("admin"));

// Token validation endpoint
app.MapGet("/validate", (HttpContext httpContext) =>
{
    var user = httpContext.User;
    var claims = user.Claims.Select(c => new { c.Type, c.Value }).ToList();

    return new
    {
        IsAuthenticated = user.Identity?.IsAuthenticated ?? false,
        Username = user.Identity?.Name,
        Claims = claims
    };
}).RequireAuthorization();

app.Run();