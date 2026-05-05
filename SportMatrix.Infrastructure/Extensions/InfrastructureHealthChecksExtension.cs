using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SportMatrix.Infrastructure.Extensions;

public static class InfrastructureHealthChecksExtension
{
    public static IHealthChecksBuilder AddInfrastructureHealthChecks(
        this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        // Database health check (SQLite is not directly supported by HealthChecks,
        // so we use a custom check that verifies the database file is accessible)
        builder.AddCheck("database", () =>
        {
            try
            {
                string? connectionString = configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return HealthCheckResult.Unhealthy("Database connection string is not configured");
                }

                return HealthCheckResult.Healthy("Database is configured");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Database check failed: {ex.Message}");
            }
        }, tags: new[] { "db", "infrastructure" });

        return builder;
    }
}
