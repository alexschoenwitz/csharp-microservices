using System;
using System.Threading;
using System.Threading.Tasks;
using BlueprintService.Data.Repositories;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BlueprintService.Configuration
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly IBlueprintRepository _repository;

        public DatabaseHealthCheck(IBlueprintRepository repository)
        {
            _repository = repository;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var isHealthy = await _repository.HealthCheckAsync(cancellationToken);

                if (isHealthy)
                {
                    return HealthCheckResult.Healthy("Database connection is healthy");
                }

                return HealthCheckResult.Degraded("Database health check failed");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database health check failed", ex);
            }
        }
    }

    public class ServiceHealthCheck : IHealthCheck
    {
        private readonly BlueprintConfiguration _config;

        public ServiceHealthCheck(BlueprintConfiguration config)
        {
            _config = config;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // Simple availability check
            // In a real-world scenario, we might check dependencies or other critical components
            if (!string.IsNullOrEmpty(_config.ServiceName))
            {
                return Task.FromResult(HealthCheckResult.Healthy("Service is configured and running"));
            }

            return Task.FromResult(HealthCheckResult.Degraded("Service has configuration issues"));
        }
    }
}
