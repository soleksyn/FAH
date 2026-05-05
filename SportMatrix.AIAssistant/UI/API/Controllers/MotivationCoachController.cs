namespace SportMatrix.AIAssistant.UI.API.Controllers;

using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MotivationCoachController : ControllerBase
{
    private readonly IMotivationCoachService motivationCoachService;
    private readonly ILogger<MotivationCoachController> logger;

    public MotivationCoachController(
        IMotivationCoachService motivationCoachService,
        ILogger<MotivationCoachController> logger)
    {
        this.motivationCoachService = motivationCoachService;
        this.logger = logger;
    }

    [HttpPost("motivate")]
    public async Task<ActionResult<MotivationResponseDto>> GetMotivation(
        [FromBody] MotivationRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation(
                "Generating motivational message for athlete: {Name}",
                request.AthleteProfile.Name);

            // Uses Gemini AI for motivation generation
            MotivationResponseDto result = await this.motivationCoachService.GenerateMotivationAsync(request, cancellationToken);
            return this.Ok(result);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error generating motivational message");
            return this.StatusCode(500, "An error occurred while generating motivational message");
        }
    }

    // Health check for the motivation service
    [HttpGet("health")]
    public async Task<ActionResult> HealthCheck(CancellationToken cancellationToken)
    {
        try
        {
            MotivationRequestDto testRequest = new MotivationRequestDto
            {
                AthleteProfile = new AthleteProfileDto
                {
                    Name = "Test User",
                    FitnessLevel = "Beginner",
                    PrimaryGoal = "Health Check",
                },
                IsStruggling = false,
            };

            MotivationResponseDto result = await this.motivationCoachService.GenerateMotivationAsync(testRequest, cancellationToken);

            return this.Ok(new
            {
                status = "healthy",
                message = "Motivation service is responding",
                timestamp = DateTime.UtcNow,
            });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Health check failed");
            return this.StatusCode(503, new
            {
                status = "unhealthy",
                message = ex.Message,
                timestamp = DateTime.UtcNow,
            });
        }
    }
}
