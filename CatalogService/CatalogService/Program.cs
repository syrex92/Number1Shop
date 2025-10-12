using CatalogService.Core.Domain.Interfaces;
using CatalogService.DataAccess.Data;
using CatalogService.DataAccess.Repositories;
using CatalogService.Interfaces;
using CatalogService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
{
    if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Test"))
    {
        options
            .UseSqlite("Data Source=catalog-service-db.db");
    }
    else
        options.UseNpgsql(builder.Configuration["CONNECTION_STRING"] ?? throw new InvalidProgramException("No connection for data base"));
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
