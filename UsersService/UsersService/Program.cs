using UsersService.Persistence;
using UsersService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddPersistanceServices(builder.Configuration);
builder.Services.AddInfastructureServices(builder.Configuration);
builder.Services.AddSwaggerGen(); // Добавляем генератор Swagger

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Включаем middleware для генерации Swagger JSON
    app.UseSwaggerUI(); // Включаем Swagger UI
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
