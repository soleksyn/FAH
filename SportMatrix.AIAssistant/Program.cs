using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.AIAssistant.Infrastructure.Configuration;
using SportMatrix.AIAssistant.Infrastructure.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure Google AI options
builder.Services.Configure<GoogleAIOptions>(
    builder.Configuration.GetSection("GoogleAI"));

// Register AI services
builder.Services.AddScoped<IAIPromptService, GoogleGeminiService>();
builder.Services.AddScoped<IMotivationCoachService, MotivationCoachService>();
builder.Services.AddScoped<IWorkoutAnalysisService, WorkoutAnalysisService>();

// Add gRPC services
builder.Services.AddGrpc();

// Add controllers for REST API
builder.Services.AddControllers();

// Add Swagger for development
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy
            .WithOrigins("http://localhost:4200")
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
app.UseCors("AllowAngularApp");
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map gRPC services
app.MapGrpcService<SportMatrix.AIAssistant.UI.API.Services.MotivationGrpcService>();
app.MapGrpcService<SportMatrix.AIAssistant.UI.API.Services.WorkoutAnalysisGrpcService>();

// Map health check endpoint
app.MapHealthChecks("/health");

app.Run();
