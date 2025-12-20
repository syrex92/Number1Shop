using CatalogService;
using CatalogService.Core.Domain.Interfaces;
using CatalogService.DataAccess.Data;
using CatalogService.DataAccess.Repositories;
using CatalogService.Helpers;
using CatalogService.Interfaces;
using CatalogService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
{
    if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Test"))
    {
        options
            .UseSqlite("Data Source=catalog-service-db.db")
            .UseSeeding((context, seed) => FakeCatalogData.SeedData(context, seed))
            .UseAsyncSeeding((context, seed, ct) => FakeCatalogData.SeedDataAsync(context, seed, ct));
    }
    else
        options.UseNpgsql(builder.Configuration["CONNECTION_STRING"] ?? throw new InvalidProgramException("No connection for data base"));
});

builder.Services.AddMassTransit(x => {
    x.UsingRabbitMq((context, cfg) =>
    {
        //x.AddConsumer<IConsumer>();
        RabbitConfigurator.ConfigureRmq(cfg, builder.Configuration);
        //RabbitConfigurator.RegisterEndPoints(cfg, context);
    });
});

builder.Services.AddScoped<IProductService, ProductsService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();

builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    var xmlPath = $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{builder.Environment.ApplicationName}.xml";
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, true);
    }
});

builder.Services.AddCors(options =>
    options.AddPolicy("myCors", policy =>
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test"))
        await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();

    //var pendingMigrations = await db.Database.GetPendingMigrationsAsync();

    //if (pendingMigrations.Any())
    //{
    //    await db.Database.MigrateAsync();
    //}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy =>
    policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        //.AllowAnyOrigin()
        .SetIsOriginAllowed(x => true)
        .AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Task.FromResult<IResult>(TypedResults.Text("Catalog service working")))
    .WithName("CheckHealth")
    .WithTags("Сервис")
    .Produces(200)
    .AllowAnonymous();

app.Run();
