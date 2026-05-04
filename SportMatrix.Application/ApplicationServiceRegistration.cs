using System.Reflection;
using SportMatrix.Application.Interfaces;
using SportMatrix.Application.Mapping;
using SportMatrix.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SportMatrix.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registriere AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddTransient<MappingProfile>();

        // Registriere Services
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IAthleteService, AthleteService>();
        services.AddScoped<ITrainingPlanService, TrainingPlanService>();

        return services;
    }
}
