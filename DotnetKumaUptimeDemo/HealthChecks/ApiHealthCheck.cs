using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotnetKumaUptimeDemo.HealthChecks;

public class ApiHealthCheck : IHealthCheck
{
    private static bool _simulateFailure = false;
    public static void SimulateFailure(bool fail) => _simulateFailure = fail;
    public bool IsHealthy() => !_simulateFailure;
    public static bool IsHealthyStatic() => !_simulateFailure;
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (_simulateFailure)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "API is unhealthy: dependency chain broken"
            ));
        }

        return Task.FromResult(HealthCheckResult.Healthy("API is operational"));
    }
}
