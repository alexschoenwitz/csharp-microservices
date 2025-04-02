using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BlueprintService.Telemetry
    {
    public static class ActivitySource
        {
        public static readonly System.Diagnostics.ActivitySource Source = new("BlueprintService");
        }

    public static class TelemetrySetup
        {
        public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
            {
            var serviceName = configuration.GetValue<string>("Blueprint:ServiceName") ?? "BlueprintService";

            // Configure resource attributes with service information
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(serviceName: serviceName, serviceVersion: "1.0.0")
                .AddTelemetrySdk()
                .AddAttributes(new[] {
                    new KeyValuePair<string, object>("deployment.environment",
                        configuration.GetValue<string>("Environment") ?? "development")
                });

            // Add OpenTelemetry with combined tracing and metrics
            services.AddOpenTelemetry()
                // Configure tracing
                .WithTracing(builder =>
                {
                    builder
                        .SetResourceBuilder(resourceBuilder)
                        .AddSource(ActivitySource.Source.Name)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();

                    // Conditionally add OTLP exporter if configured
                    var otlpEndpoint = configuration.GetValue<string>("Telemetry:OtlpEndpoint");
                    if (!string.IsNullOrEmpty(otlpEndpoint))
                        {
                        builder.AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(otlpEndpoint);
                        });
                        }
                })
                // Configure metrics
                .WithMetrics(builder =>
                {
                    builder
                        .SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation()
                        .AddRuntimeInstrumentation();

                    // Conditionally add OTLP exporter if configured
                    var otlpEndpoint = configuration.GetValue<string>("Telemetry:OtlpEndpoint");
                    if (!string.IsNullOrEmpty(otlpEndpoint))
                        {
                        builder.AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(otlpEndpoint);
                        });
                        }
                });

            return services;
            }
        }
    }
