using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using StackExchange.Redis;
using System.Text.Json;
using DotnetKumaUptimeDemo.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ApiHealthCheck>();
builder.Services.AddHealthChecks()
    .AddCheck<PostgresHealthCheck>("postgres", tags: new[] { "database" })
    .AddCheck<RedisHealthCheck>("redis", tags: new[] { "cache" })
    .AddCheck<ApiHealthCheck>("api", tags: new[] { "core" });

if(builder.Configuration.GetValue<bool>("Redis:Enabled"))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]
            ?? "localhost:6379"
    ));   
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = 200,
        [HealthStatus.Degraded] = 503,
        [HealthStatus.Unhealthy] = 503
    },
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                component = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds,
                tags = e.Value.Tags,
                data = e.Value.Data
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            WriteIndented = true,
        }));
    }
});

app.MapHealthChecks("/health/database", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("database")
});

app.MapHealthChecks("/health/cache", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("cache")
});

// Simulate API endpoints that can fail
app.MapGet("/api/users", () =>
{
    if (!ApiHealthCheck.IsHealthyStatic())
        return Results.StatusCode(503);

    return Results.Ok(new[] { new { id = 1, name = "John Doe" } });
});

app.MapGet("/api/status", () => Results.Ok(new
{
    service = "DotnetKumaDemo",
    version = "1.0.0",
    timestamp = DateTime.UtcNow
}));

app.MapPost("/api/simulate/failure", (string component) =>
{
    if (component == "postgres")
        PostgresHealthCheck.SimulateFailure(true);
    else if (component == "redis")
        RedisHealthCheck.SimulateFailure(true);
    else if (component == "api")
        ApiHealthCheck.SimulateFailure(true);
    else
        return Results.BadRequest(new { error = "Invalid component. Use: postgres, redis, api" });

    return Results.Ok(new { message = $"Simulated failure for {component}" });
});

app.MapPost("/api/simulate/recovery", (string component) =>
{
    if (component == "postgres")
        PostgresHealthCheck.SimulateFailure(false);
    else if (component == "redis")
        RedisHealthCheck.SimulateFailure(false);
    else if (component == "api")
        ApiHealthCheck.SimulateFailure(false);
    else
        return Results.BadRequest(new { error = "Invalid component. Use: postgres, redis, api" });

    return Results.Ok(new { message = $"Recovered {component}" });
});

app.Run();