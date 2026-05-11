using SportMatrix.Application.Interfaces;
using SportMatrix.Infrastructure.Configuration;
using SportMatrix.Infrastructure.Extensions;
using SportMatrix.Infrastructure.Persistence;
using SportMatrix.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
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

        // ASP.NET Core Identity
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit           = true;
            options.Password.RequiredLength          = 6;
            options.Password.RequireNonAlphanumeric  = false;
            options.Password.RequireUppercase        = false;
            options.Password.RequireLowercase        = true;
            options.SignIn.RequireConfirmedAccount   = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Configure cookie paths for the MVC frontend
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath       = "/Account/Login";
            options.LogoutPath      = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

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

