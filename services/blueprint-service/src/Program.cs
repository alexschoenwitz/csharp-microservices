using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlueprintService.Configuration;
using BlueprintService.Data.Repositories;
using BlueprintService.Services;
using BlueprintService.Telemetry;
using BlueprintService.Validation;

namespace BlueprintService
    {
    public class Program
        {
        public static void Main(string[] args)
            {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();
            ConfigureApplication(app);

            app.Run();
            }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
            {
            // Bind and validate configuration
            var blueprintConfig = new BlueprintConfiguration();
            configuration.GetSection("Blueprint").Bind(blueprintConfig);
            ConfigurationValidator.ValidateConfiguration(blueprintConfig);
            services.AddSingleton(blueprintConfig);

            // Database configuration
            var databaseConfig = new DatabaseConfiguration();
            configuration.GetSection("Database").Bind(databaseConfig);
            ConfigurationValidator.ValidateConfiguration(databaseConfig);
            services.AddSingleton(databaseConfig);

            // Register repositories
            services.AddSingleton<IBlueprintRepository, BlueprintRepository>();

            // Register services
            services.AddSingleton<IBlueprintService, BlueprintService.Services.BlueprintService>();

            // Configure gRPC Service
            services.AddGrpc();

            // Register health checks
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("database_health")
                .AddCheck<ServiceHealthCheck>("service_health");

            // Configure OpenTelemetry
            services.AddTelemetry(configuration);
            }

        private static void ConfigureApplication(WebApplication app)
            {
            // Configure middleware pipeline
            if (app.Environment.IsDevelopment())
                {
                app.UseDeveloperExceptionPage();
                }

            // Configure routing and endpoints
            app.UseRouting();

            app.MapGrpcService<GrpcServices.BlueprintGrpcService>();
            app.MapHealthChecks("/health");

            // Add global error handling middleware
            app.UseMiddleware<ErrorHandlingMiddleware>();
            }
        }
    }
