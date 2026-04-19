using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotnetKumaUptimeDemo.HealthChecks;

public class PostgresHealthCheck : IHealthCheck
{
    private static bool _simulateFailure = false;
    public static void SimulateFailure(bool fail) => _simulateFailure = fail;
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (_simulateFailure)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "PostgreSQL connection failed: timeout after 30s",
                data: new Dictionary<string, object> { { "error_code", "DB_TIMEOUT" } }
            ));
        }

        return Task.FromResult(HealthCheckResult.Healthy(
            "PostgreSQL is healthy",
            data: new Dictionary<string, object>
            {
                { "latency_ms", Random.Shared.Next(5, 50) },
                { "connections", Random.Shared.Next(10, 100) }
            }
        ));
    }
}
