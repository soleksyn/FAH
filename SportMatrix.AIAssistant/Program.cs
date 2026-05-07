using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.AIAssistant.Infrastructure.Configuration;
using SportMatrix.AIAssistant.Infrastructure.Providers;
using SportMatrix.AIAssistant.Infrastructure.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure Google AI options
builder.Services.Configure<GoogleAIOptions>(
    builder.Configuration.GetSection("GoogleAI"));
builder.Services.Configure<SportMatrixApiOptions>(
    builder.Configuration.GetSection(SportMatrixApiOptions.SectionName));

// Register AI services
builder.Services.AddScoped<IAIPromptService, GoogleGeminiService>();

builder.Services.AddScoped<IWorkoutAnalysisService, WorkoutAnalysisService>();
builder.Services.AddHttpClient<IWorkoutDataProvider, SportMatrixWorkoutDataProvider>(
    (serviceProvider, httpClient) =>
    {
        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
        string baseUrl = configuration[$"{SportMatrixApiOptions.SectionName}:BaseUrl"] ?? "https://localhost:44333";
        httpClient.BaseAddress = new Uri(baseUrl);
    });

// Add gRPC services
builder.Services.AddGrpc();

// Add controllers for REST API
builder.Services.AddControllers();

// Add Swagger for development
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    string[] allowedOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>()
        ?? ["http://localhost:5000", "https://localhost:5001"];

    options.AddPolicy("AllowSportMatrixFrontend",
        policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add health checks
builder.Services.AddHealthChecks();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSportMatrixFrontend");
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map gRPC services

app.MapGrpcService<SportMatrix.AIAssistant.UI.API.Services.WorkoutAnalysisGrpcService>();

// Map health check endpoint
app.MapHealthChecks("/health");

app.Run();
