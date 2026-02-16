using System.Security.Claims;
using CartService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Shop.CartService;
using Shop.CartService.Abstractions;
using Shop.CartService.Dto;
using Shop.CartService.Extensions;
using Shop.CartService.Model;
using Shop.CartService.Repositories;
using Shop.CartService.Services;
using Shop.Core.Messages;
using Shop.Demo.Data;

var items = ShopFakeData.Products;
Console.WriteLine(items);
    

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSerilog(options =>
    {
        options.MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
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
                .UseSqlite("Data Source=cart-service-db.db")
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



builder.Services.AddScoped<IRepository<Cart>, EfRepository<Cart>>();
builder.Services.AddScoped<IRepository<CartItem>, EfRepository<CartItem>>();
builder.Services.AddScoped<IProductSearchRepository, ProductSearchRepository>();

builder.Services.AddSingleton<IMessageListener<ProductMessage>, RabbitMqMessageListener<ProductMessage>>();
// TODO: enable
//builder.Services.AddHostedService<RabbitBackgroundService>();

builder.Services.Configure<RabbitMqOptions>(options =>
{
    options.Host = builder.Configuration["RMQ_HOST"] ?? string.Empty;
    options.Port = int.TryParse(builder.Configuration["RMQ_PORT"] ?? string.Empty, out int port) ? port : 0;
    options.UserName = builder.Configuration["RMQ_USER"] ?? string.Empty;
    options.Password = builder.Configuration["RMQ_PASSWORD"] ?? string.Empty;
    options.CartProductMessagesQueue = builder.Configuration["RMQ_CART_PRODUCT_QUEUE"] ?? string.Empty;
    options.ProductMessagesExchange = builder.Configuration["RMQ_PRODUCT_EXCHANGE"] ?? string.Empty;

    /*
    options.Host = "localhost";
    options.Port = 5672;
    options.UserName = "guest";
    options.Password = "guest";
    options.CartProductMessagesQueue = "cart-product-messages-queue";
    options.ProductMessagesExchange = "product-messages-exchange";
     */
});

builder.Services.AddCors(options => 
    options.AddPolicy("debug", policy => 
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()));

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
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop Cart API v1"));
}

app.UseCors(policy => 
    policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        //.AllowAnyOrigin()
        .SetIsOriginAllowed(x => true)
        .AllowCredentials());

app.UseHttpsRedirection();


var jwtAuthorizationPolicy = new AuthorizationPolicyBuilder()
    .RequireAssertion(c => c.User.Claims.Any(x => x.Type == ClaimTypes.Sid))
    .RequireAuthenticatedUser()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .Build();

//app.MapPost() - не нужен, создание корзины происходит автоматически при первом обращении

app.MapGet("/", CartRequestsHandler.GetCart)
    .RequireAuthorization(jwtAuthorizationPolicy)
    .WithName("GetCart")
    .WithTags("Корзина")
    .Produces<CartResponse>(200, "application/json")
    .Produces(401)
    .WithOpenApi();

app.MapPost("/", CartRequestsHandler.AddToCart)
    .RequireAuthorization(jwtAuthorizationPolicy)
    .WithName("AddToCart")
    .WithTags("Корзина")
    .Produces<CartResponse>(200, "application/json")
    .Produces(401)
    .Produces(201)
    .WithOpenApi();


app.MapPut("/", CartRequestsHandler.UpdateCart)
    .RequireAuthorization(jwtAuthorizationPolicy)
    .WithName("UpdateCart")
    .WithTags("Корзина")
    .Produces<CartResponse>(200, "application/json")
    .Produces(401)
    .Produces(404)
    .WithOpenApi();

app.MapDelete("/", CartRequestsHandler.ClearCart)
    .RequireAuthorization(jwtAuthorizationPolicy)
    .WithName("ClearCart")
    .WithTags("Корзина")
    .Produces(200)
    .Produces(401)
    .Produces(404)
    .WithOpenApi();

app.MapGet("/health", CartRequestsHandler.CheckHealth)
    .WithName("CheckHealth")
    .WithTags("Сервис")
    .Produces(200)
    .AllowAnonymous()
    .WithOpenApi();

app.Run();

/// <summary>
/// 
/// </summary>
public abstract partial class Program
{
}