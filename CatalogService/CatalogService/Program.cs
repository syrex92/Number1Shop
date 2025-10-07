using CatalogService.Api.Interfaces;
using CatalogService.Api.Services;
using CatalogService.Core.Domain.Interfaces;
using CatalogService.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IProductService, ProductsService>();

builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
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
