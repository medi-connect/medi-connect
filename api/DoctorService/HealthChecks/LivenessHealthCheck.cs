using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DoctorService.HealthChecks;

public class LivenessHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}