using SportMatrix.Application.DTOs;
using SportMatrix.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SportMatrix.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly IAIAssistantClientService aiAssistant;
    private readonly ILogger<AIController> logger;

    public AIController(
        IAIAssistantClientService aiAssistant,
        ILogger<AIController> logger)
    {
        this.aiAssistant = aiAssistant;
        this.logger = logger;
    }


    [HttpPost("analysis")]
    public async Task<ActionResult<AIWorkoutAnalysisResponseDto>> GetWorkoutAnalysis(
        AIWorkoutAnalysisRequestDto request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "AI workout analysis request for {WorkoutCount} workouts, type: {AnalysisType}",
            request.RecentWorkouts?.Count ?? 0, request.AnalysisType ?? "General");

        AIWorkoutAnalysisResponseDto result = await aiAssistant.GetWorkoutAnalysisAsync(request, cancellationToken);

        logger.LogInformation("AI analysis response generated from source: {Source}", result.Source);

        return Ok(result);
    }

    [HttpGet("health")]
    public async Task<ActionResult<object>> GetAIHealth(CancellationToken cancellationToken)
    {
        bool isHealthy = await this.aiAssistant.IsHealthyAsync(cancellationToken);

        return this.Ok(new
        {
            isHealthy = isHealthy,
            service = "AIAssistant",
            timestamp = DateTime.UtcNow,
            status = isHealthy ? "Available" : "Unavailable",
        });
    }
}