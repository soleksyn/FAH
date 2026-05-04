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

        // Register DbContext Interface
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Service registrieren
        services.AddScoped<IAIAssistantClientService, AIAssistantClientService>();

        // HttpClient für AIAssistant registrieren
        services.AddHttpClient<IAIAssistantClientService, AIAssistantClientService>(client =>
        {
            client.BaseAddress = new Uri(configuration["AIAssistant:BaseUrl"] ?? "http://localhost:5169");
        });

        // HealthChecks für die Infrastruktur hinzufügen
        services.AddHealthChecks()
            .AddInfrastructureHealthChecks(configuration);

        return services;
    }
}
