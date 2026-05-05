namespace SportMatrix.AIAssistant.UI.API.Controllers;

using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.AIAssistant.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly ILogger<DebugController> logger;

    public DebugController(IConfiguration configuration, ILogger<DebugController> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }

    [HttpGet("config-check")]
    public ActionResult ConfigCheck()
    {
        string? apiKey = this.configuration["GoogleAI:ApiKey"];
        return this.Ok(new
        {
            hasApiKey = !string.IsNullOrEmpty(apiKey),
            apiKeyLength = apiKey?.Length ?? 0,
            apiKeyPreview = !string.IsNullOrEmpty(apiKey) && apiKey.Length > 10 ? $"{apiKey.Substring(0, 10)}..." : "NULL",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        });
    }

    [HttpGet("test-gemini-service")]
    public async Task<ActionResult> TestGeminiService(CancellationToken cancellationToken)
    {
        try
        {
            IMotivationCoachService motivationService = this.HttpContext.RequestServices.GetRequiredService<IMotivationCoachService>();

            MotivationRequestDto testRequest = new MotivationRequestDto
            {
                AthleteProfile = new AthleteProfileDto
                {
                    Name = "TestUser",
                    FitnessLevel = "Intermediate",
                    PrimaryGoal = "General Fitness"
                }
            };

            var result = await motivationService.GenerateMotivationAsync(testRequest, cancellationToken);

            return this.Ok(new
            {
                message = "Gemini Service Test",
                hasMotivation = !string.IsNullOrEmpty(result.MotivationalMessage),
                generatedAt = result.GeneratedAt,
                response = result
            });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Gemini Service test failed");
            return this.StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("health")]
    public ActionResult HealthCheck()
    {
        return this.Ok(new
        {
            status = "healthy",
            message = "Debug controller is responding",
            timestamp = DateTime.UtcNow
        });
    }
}
