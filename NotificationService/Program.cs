using NotificationService.Hubs;
using NotificationService.Services.RabbitMq;
using NotificationService.Services.Realtime;
using RabbitMqService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton(sp =>
{
    var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMqOptions>>().Value;
    return new RabbitMqClientOptions
    {
        HostName = opts.Host,
        Port = opts.Port,
        UserName = opts.User,
        Password = opts.Password,
        VirtualHost = opts.VirtualHost,
        PrefetchCount = 10
    };
});

builder.Services.AddSignalR();
builder.Services.AddHostedService<NotificationConsumerHostedService>();
builder.Services.AddSingleton<ConnectedUsersTracker>();
builder.Services.AddSingleton<PendingNotificationsStore>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials());
});

var app = builder.Build();

app.UseCors();

// Lightweight auth for SignalR: trust JWT claims without signature validation.
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/hubs/notifications", StringComparison.OrdinalIgnoreCase) &&
        ctx.Request.Query.TryGetValue("access_token", out var tokenValues))
    {
        var token = tokenValues.ToString();
        if (!string.IsNullOrWhiteSpace(token))
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var sid = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value?.Trim();
                var nameId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value?.Trim();
                if (!string.IsNullOrWhiteSpace(sid) || !string.IsNullOrWhiteSpace(nameId))
                {
                    var identity = new ClaimsIdentity(jwt.Claims, "jwt");
                    ctx.User = new ClaimsPrincipal(identity);
                }
            }
            catch
            {
                // ignore invalid token
            }
        }
    }

    await next();
});

app.MapGet("/health", () => Results.Ok(new { Status = "ok" }));
app.MapHub<NotificationsHub>("/hubs/notifications");

app.Run();

