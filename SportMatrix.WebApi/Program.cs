using System.Text.Json;
using SportMatrix.Application;
using SportMatrix.Application.Interfaces;
using SportMatrix.Infrastructure;
using SportMatrix.Infrastructure.Services;

using SportMatrix.WebApi.Middleware;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Learn more about configuring Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Fitness Analytics Hub API",
        Version = "v1",
        Description = "Backend API f?r die FitnessAnalyticsHub-Anwendung",
    });
});

// Register application services
builder.Services.AddApplication();

// Register infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Base HealthChecks
builder.Services.AddHealthChecks()
    .AddCheck("api", () => HealthCheckResult.Healthy(), tags: new[] { "service" });

// UI f?r HealthChecks
builder.Services.AddHealthChecksUI(setup =>
{
    setup.SetEvaluationTimeInSeconds(60); // Alle 60 Sekunden pr?fen
    setup.MaximumHistoryEntriesPerEndpoint(50); // 50 Eintr?ge in der Historie speichern
})
.AddInMemoryStorage();



// ==========================================
// AIAssistant Client Services Registrierung
// ==========================================

// Alle drei Client-Implementierungen registrieren
builder.Services.AddHttpClient<AIAssistantClientService>();
builder.Services.AddScoped<GrpcAIAssistantClientService>();
builder.Services.AddHttpClient<GrpcJsonClientService>();

// Konfigurierbare Service-Auswahl basierend auf appsettings.json
builder.Services.AddScoped<IAIAssistantClientService>(provider =>
{
    IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
    string clientType = configuration["AIAssistant:ClientType"] ?? "Http";

    ILogger<Program> logger = provider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Using AIAssistant ClientType: {ClientType}", clientType);

    return clientType.ToLower() switch
    {
        "http" => provider.GetRequiredService<AIAssistantClientService>(),
        "grpc" => provider.GetRequiredService<GrpcAIAssistantClientService>(),
        "grpcjson" => provider.GetRequiredService<GrpcJsonClientService>(),
        _ => throw new InvalidOperationException($"Unknown AIAssistant ClientType: {clientType}. Valid values: Http, Grpc, GrpcJson")
    };
});


WebApplication app = builder.Build();

// Exception Handling
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fitness Analytics Hub API v1");
    c.RoutePrefix = string.Empty; // Um Swagger als Startseite zu setzen
});

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

// HealthChecks Endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});

// Gruppierte HealthChecks nach Tags
app.MapHealthChecks("/health/infrastructure", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("infrastructure"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});

// Health UI Dashboard
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
});

app.MapControllers();

// Zeige beim Start an, welcher AIAssistant Client verwendet wird
ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
IConfiguration config = app.Services.GetRequiredService<IConfiguration>();
string clientType = config["AIAssistant:ClientType"] ?? "Http";
logger.LogInformation("AIAssistant Client Type: {ClientType}", clientType);

app.Run();
