using SportMatrix.Application.Interfaces;
using SportMatrix.Infrastructure.Configuration;
using SportMatrix.Infrastructure.Extensions;
using SportMatrix.Infrastructure.Persistence;
using SportMatrix.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SportMatrix.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database configuration
        services.AddDbContext<ApplicationDbContext>(options =>
                DatabaseConfiguration.ConfigureDatabase(options, configuration));

        // Register DbContext interface
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Register AIAssistant HTTP client
        services.AddHttpClient<IAIAssistantClientService, AIAssistantClientService>(client =>
        {
            client.BaseAddress = new Uri(configuration["AIAssistant:BaseUrl"] ?? "http://localhost:5169");
        });

        // Add infrastructure health checks
        services.AddHealthChecks()
            .AddInfrastructureHealthChecks(configuration);

        return services;
    }
}
