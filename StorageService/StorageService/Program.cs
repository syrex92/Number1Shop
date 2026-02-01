using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StorageService.DTO;
using StorageService.Extensions;
using StorageService.Handlers;
using StorageService.Interfaces;
using StorageService.Models;
using StorageService.Repository;
using StorageService.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
                .UseSqlite("Data Source=stock-service-db.db")
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
builder.Services.AddScoped<IInventoryService, InventoryService>();

// CORS для отладки
builder.Services.AddCors(options =>
    options.AddPolicy("debug", policy =>
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()));

var app = builder.Build();

// Инициализация базы данных
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
        .SetIsOriginAllowed(x => true)
        .AllowCredentials());

app.UseHttpsRedirection();

// Политика авторизации
var jwtAuthorizationPolicy = new AuthorizationPolicyBuilder()
    .RequireAssertion(c => c.User.Claims.Any(x => x.Type == ClaimTypes.Sid))
    .RequireAuthenticatedUser()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .Build();

// ================ МАРШРУТЫ API ================

// Проверка доступности товара
app.MapGet("/availability/{productId:guid}", InventoryRequestsHandler.CheckAvailability)
    .RequireAuthorization(jwtAuthorizationPolicy)
    .WithName("CheckAvailability")
    .WithTags("Склад")
    .Produces<StockAvailabilityResponse>(200, "application/json")
    .Produces(401)
    .Produces(404)
    .WithOpenApi(operation =>
    {
        operation.Parameters[0].Description = "ID товара для проверки наличия";
        operation.Parameters[1].Description = "Запрашиваемое количество";
        operation.Summary = "Проверить доступность товара на складе";
        return operation;
    });

// Резервирование товара
app.MapPost("/reserve", InventoryRequestsHandler.ReserveStock)
    .RequireAuthorization(jwtAuthorizationPolicy)
    .WithName("ReserveStock")
    .WithTags("Склад")
    .Produces<ReservationResponse>(200, "application/json")
    .Produces(400)
    .Produces(401)
    .WithOpenApi(operation =>
    {
        operation.Summary = "Зарезервировать товары на складе для заказа";
        operation.Description = "Резервирует указанные товары на складе на 30 минут для оформления заказа";
        return operation;
    });

// Подтверждение покупки (списание)
app.MapPost("/confirm", InventoryRequestsHandler.ConfirmPurchase)
    .RequireAuthorization(jwtAuthorizationPolicy)
    .WithName("ConfirmPurchase")
    .WithTags("Склад")
    .Produces<PurchaseConfirmationResponse>(200, "application/json")
    .Produces(400)
    .Produces(401)
    .WithOpenApi(operation =>
    {
        operation.Summary = "Подтвердить покупку и списать товары со склада";
        operation.Description = "Подтверждает покупку после успешной оплаты заказа";
        return operation;
    });

// Отмена резервирования
app.MapDelete("/reserve/{reservationId:guid}", InventoryRequestsHandler.CancelReservation)
    .RequireAuthorization(jwtAuthorizationPolicy)
    .WithName("CancelReservation")
    .WithTags("Склад")
    .Produces(200)
    .Produces(400)
    .Produces(401)
    .Produces(404)
    .WithOpenApi(operation =>
    {
        operation.Summary = "Отменить резервирование товаров";
        operation.Description = "Освобождает зарезервированные товары и возвращает их на склад";
        return operation;
    });

// Получение информации о товаре на складе
app.MapGet("/stock/{productId:guid}", InventoryRequestsHandler.GetStockItem)
    .RequireAuthorization(jwtAuthorizationPolicy)
    .WithName("GetStockItem")
    .WithTags("Склад")
    .Produces<StockItem>(200, "application/json")
    .Produces(401)
    .Produces(404)
    .WithOpenApi(operation =>
    {
        operation.Summary = "Получить информацию о товаре на складе";
        return operation;
    });

// Health check
app.MapGet("/health", InventoryRequestsHandler.CheckHealth)
    .WithName("CheckHealth")
    .WithTags("Сервис")
    .Produces(200)
    .AllowAnonymous()
    .WithOpenApi();

app.Run();

/// <summary>
/// Точка входа программы
/// </summary>
public abstract partial class Program;
