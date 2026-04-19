using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotnetKumaUptimeDemo.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private static bool _simulateFailure = false;
    public static void SimulateFailure(bool fail) => _simulateFailure = fail;
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (_simulateFailure)
        {
            return Task.FromResult(HealthCheckResult.Degraded(
                "Redis connection slow: 5000ms response time",
                data: new Dictionary<string, object> { { "latency_ms", 5000 } }
            ));
        }

        return Task.FromResult(HealthCheckResult.Healthy(
            "Redis is healthy",
            data: new Dictionary<string, object>
            {
                { "latency_ms", Random.Shared.Next(1, 10) },
                { "memory_usage_mb", Random.Shared.Next(50, 200) }
            }
        ));
    }
}
