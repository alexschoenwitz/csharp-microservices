using System;
using System.Collections.Generic;
using BlueprintService.Configuration;

namespace BlueprintService.Validation
    {
    public static class ConfigurationValidator
        {
        public static void ValidateConfiguration(BlueprintConfiguration config)
            {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(config.ServiceName)) { errors.Add("ServiceName cannot be empty"); }

            if (config.RetryAttempts < 0) { errors.Add($"RetryAttempts must be non-negative, but was {config.RetryAttempts}"); }

            if (config.RequestTimeout <= TimeSpan.Zero) { errors.Add($"RequestTimeout must be positive, but was {config.RequestTimeout}"); }

            if (config.MaxConcurrentRequests <= 0) { errors.Add($"MaxConcurrentRequests must be positive, but was {config.MaxConcurrentRequests}"); }

            if (errors.Count != 0)
                {
                throw new ArgumentException($"Invalid configuration: {string.Join(", ", errors)}");
                }
            }

        public static void ValidateConfiguration(DatabaseConfiguration config)
            {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(config.ConnectionString))
                {
                errors.Add("ConnectionString cannot be empty");
                }

            if (config.CommandTimeout <= 0)
                {
                errors.Add($"CommandTimeout must be positive, but was {config.CommandTimeout}");
                }

            if (config.MaxPoolSize <= 0)
                {
                errors.Add($"MaxPoolSize must be positive, but was {config.MaxPoolSize}");
                }

            if (config.ConnectionIdleTimeout <= TimeSpan.Zero)
                {
                errors.Add($"ConnectionIdleTimeout must be positive, but was {config.ConnectionIdleTimeout}");
                }

            if (errors.Count != 0)
                {
                throw new ArgumentException($"Invalid database configuration: {string.Join(", ", errors)}");
                }
            }
        }
    }
