using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Shop.FavoriteService;
using Shop.FavoriteService.Abstractions;
using Shop.FavoriteService.Dto;
using Shop.FavoriteService.Extensions;
using Shop.FavoriteService.Model;
using Shop.FavoriteService.Repositories;
using Shop.FavoriteService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSerilog(options =>
    {
        options
            .MinimumLevel.Is(Enum.Parse<LogEventLevel>(builder.Configuration["Logging:LogLevel:Default"] ?? "Debug"))
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .Filter.ByExcluding(ctx => ctx.Properties.ContainsKey("RequestPath") &&
                                       ctx.Properties["RequestPath"].ToString() == "\"/health\"")
            .Filter.ByExcluding(ctx => ctx.Properties.ContainsKey("RequestPath") &&
                                       ctx.Properties["RequestPath"].ToString() == "\"/favicon.ico\"");
    })
    .AddOpenApi()
    .AddSecuredSwagger()
    .AddJwtAuthentication()
    .AddAuthorization()
    .AddDbContext<EfContext>(options =>
    {
        if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Test"))
        {
            options
                .UseSqlite("Data Source=favorite-service-db.db")
                .UseSeeding((contextToSeed, _) => EfContext.SeedData(contextToSeed))
                .UseAsyncSeeding((contextToSeed, _, _) =>
                {
                    EfContext.SeedData(contextToSeed);
                    return Task.CompletedTask;
                });
        }
        else
            options.UseNpgsql(builder.Configuration["CONNECTION_STRING"]);
    });


builder.Services.AddScoped<IRepository<FavoriteList>, EfRepository<FavoriteList>>();
builder.Services.AddScoped<IRepository<FavoriteItem>, EfRepository<FavoriteItem>>();
builder.Services.AddScoped<IProductSearchRepository, ProductSearchRepository>();

// uncomment to use RabbitMQ (is it obligatory?)
//builder.Services.AddSingleton<IMessageListener<ProductMessage>, RabbitMqMessageListener<ProductMessage>>();
//builder.Services.AddHostedService<RabbitBackgroundService>();

builder.Services.Configure<RabbitMqOptions>(options =>
{
    options.Host = builder.Configuration["RMQ_HOST"] ?? string.Empty;
    options.Port = int.TryParse(builder.Configuration["RMQ_PORT"] ?? string.Empty, out int port) ? port : 0;
    options.UserName = builder.Configuration["RMQ_USER"] ?? string.Empty;
    options.Password = builder.Configuration["RMQ_PASSWORD"] ?? string.Empty;
    options.FavoriteProductMessagesQueue = builder.Configuration["RMQ_CART_PRODUCT_QUEUE"] ?? string.Empty;
    options.ProductMessagesExchange = builder.Configuration["RMQ_PRODUCT_EXCHANGE"] ?? string.Empty;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<EfContext>();
    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test"))
        context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop FavoriteList API v1"));
}

app.UseHttpsRedirection();

var jwtAuthorizationPolicy = new AuthorizationPolicyBuilder()
    .RequireAssertion(c => c.User.Claims.Any(x => x.Type == ClaimTypes.Sid))
    .RequireAuthenticatedUser()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .Build();

app.MapGet("/", FavoriteRequestsHandler.GetFavoriteProducts)
    .RequireAuthorization(jwtAuthorizationPolicy)
    .WithName("GetFavoriteProducts")
    .WithTags("Избранное")
    .Produces<FavoriteProductListResponse>(200, "application/json")
    .Produces(401)
    .WithOpenApi();

app.MapPost("/", FavoriteRequestsHandler.AddFavoriteProduct)
    .RequireAuthorization(jwtAuthorizationPolicy)
    .WithName("AddFavoriteProduct")
    .WithTags("Избранное")
    .Produces(201)
    .Produces(401)
    .Produces(404)
    .WithOpenApi();

app.MapDelete("/{productId:guid}", FavoriteRequestsHandler.RemoveFavoriteProduct)
    .RequireAuthorization(jwtAuthorizationPolicy)
    .WithName("RemoveFavoriteProduct")
    .WithTags("Избранное")
    .Produces(200)
    .Produces(401)
    .Produces(404)
    .WithOpenApi();

app.MapDelete("/", FavoriteRequestsHandler.ClearFavoriteProducts)
    .RequireAuthorization(jwtAuthorizationPolicy)
    .WithName("ClearFavoriteProducts")
    .WithTags("Избранное")
    .Produces(200)
    .Produces(401)
    .WithOpenApi();

app.MapGet("/health", FavoriteRequestsHandler.CheckHealth)
    .WithName("CheckHealth")
    .WithTags("Сервис")
    .Produces(200)
    .AllowAnonymous()
    .WithOpenApi();

app.Run();


/// <summary>
/// 
/// </summary>
public abstract partial class Program;
