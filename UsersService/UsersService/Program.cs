using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using System.Reflection;
using UsersService.Extensions;
using UsersService.Infrastructure;
using UsersService.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Host.AddLoggerServices(builder.Configuration, builder.Environment); // ����������� Log
builder.Services.AddPersistanceServices(builder.Configuration, builder.Environment);
builder.Services.AddInfastructureServices(builder.Configuration, builder.Environment);
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Authentication API",
        Version = "v1",
        Description = "JWT Authentication Microservice"
    });

    // ��������� JWT ��������� � Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Token in format: Bearer [token]"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });

    // �������� XML ����������� ��� ���� ������
    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
    foreach (var xmlFile in xmlFiles)
    {
        options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
    }
});
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API v1");
        options.OAuthClientId("swagger-ui");
        options.OAuthAppName("Swagger UI");
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}
// ������������� ���� ������
await app.InitializeDatabaseAsync();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Global Error Handling Endpoint
app.Map("/error", () => Results.Problem("An error occurred.", statusCode: 500));

app.Run();

/// <summary>
/// ��� ������
/// </summary>
public abstract partial class Program { };