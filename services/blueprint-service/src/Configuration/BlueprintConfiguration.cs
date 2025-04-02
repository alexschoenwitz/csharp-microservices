using System;

namespace BlueprintService.Configuration
{
    public record BlueprintConfiguration
    {
        public string ServiceName { get; init; } = "BlueprintService";
        public int RetryAttempts { get; init; } = 3;
        public TimeSpan RequestTimeout { get; init; } = TimeSpan.FromSeconds(30);
        public int MaxConcurrentRequests { get; init; } = 100;
    }

    public record DatabaseConfiguration
    {
        public string ConnectionString { get; init; } = string.Empty;
        public int CommandTimeout { get; init; } = 30;
        public int MaxPoolSize { get; init; } = 100;
        public TimeSpan ConnectionIdleTimeout { get; init; } = TimeSpan.FromMinutes(5);
    }
}
