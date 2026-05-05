using Microsoft.Extensions.Options;
using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.AIAssistant.Infrastructure.Configuration;
using SportMatrix.AIAssistant.Infrastructure.Services;
using SportMatrix.AIAssistant.UI.API.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add Options
builder.Services.Configure<GoogleAIOptions>(builder.Configuration.GetSection(GoogleAIOptions.SectionName));

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Fitness Analytics Hub - AI Assistant",
        Version = "v1",
        Description = "AI-powered fitness analytics via Gemini",
    });
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// AI Service (uses Google.GenAI SDK internally)
builder.Services.AddScoped<IAIPromptService, GoogleGeminiService>();

// gRPC Services
builder.Services.AddGrpc();

// Application Services
builder.Services.AddScoped<IMotivationCoachService, MotivationCoachService>();
builder.Services.AddScoped<IWorkoutAnalysisService, WorkoutAnalysisService>();

// Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

WebApplication app = builder.Build();

// gRPC Services
app.MapGrpcService<MotivationGrpcService>();
app.MapGrpcService<WorkoutAnalysisGrpcService>();

// gRPC-Reflection
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Assistant API v1");
    });
}

app.UseExceptionHandler("/error");

// app.UseHttpsRedirection(); // Disabled to allow HTTP connections from frontend
app.UseCors("AllowAngularApp");
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new
{
    status = "healthy",
    service = "AI Assistant",
    timestamp = DateTime.UtcNow,
});

Console.WriteLine("AI Assistant Service starting...");
Console.WriteLine("Using Google Gemini for AI processing");

app.Run();
