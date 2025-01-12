using Microsoft.Extensions.Diagnostics.HealthChecks;
using UserService.Utils;

namespace UserService.Controllers;

public class DbHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext dbContext;

    public DbHealthCheck(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            // Testing DB Conn.
            if (await dbContext.Database.CanConnectAsync(cancellationToken))
            {
                return HealthCheckResult.Healthy();
            }
            return HealthCheckResult.Unhealthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy();
        }    
    }
}