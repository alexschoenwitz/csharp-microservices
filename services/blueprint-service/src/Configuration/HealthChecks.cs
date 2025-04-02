using System;
using System.Threading;
using System.Threading.Tasks;
using BlueprintService.Data.Repositories;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BlueprintService.Configuration
    {
    public class DatabaseHealthCheck(IBlueprintRepository repository) : IHealthCheck
        {
        private readonly IBlueprintRepository _repository = repository;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
            try
                {
                bool isHealthy = await _repository.HealthCheckAsync(cancellationToken).ConfigureAwait(false);

                return isHealthy
                    ? HealthCheckResult.Healthy("Database connection is healthy")
                    : HealthCheckResult.Degraded("Database health check failed");
                }
            catch (Exception ex)
                {
                return HealthCheckResult.Unhealthy("Database health check failed", ex);
                }
            }
        }

    public class ServiceHealthCheck(BlueprintConfiguration config) : IHealthCheck
        {
        private readonly BlueprintConfiguration _config = config;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
            // Simple availability check
            // In a real-world scenario, we might check dependencies or other critical components
            return !string.IsNullOrEmpty(_config.ServiceName)
                ? Task.FromResult(HealthCheckResult.Healthy("Service is configured and running"))
                : Task.FromResult(HealthCheckResult.Degraded("Service has configuration issues"));
            }
        }
    }
