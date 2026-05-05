namespace SportMatrix.AIAssistant.UI.API.Controllers;

using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.AIAssistant.Infrastructure.Providers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WorkoutAnalysisController : ControllerBase
{
    private readonly IWorkoutAnalysisService workoutAnalysisService;
    private readonly ILogger<WorkoutAnalysisController> logger;

    public WorkoutAnalysisController(
        IWorkoutAnalysisService workoutAnalysisService,
        ILogger<WorkoutAnalysisController> logger)
    {
        this.workoutAnalysisService = workoutAnalysisService;
        this.logger = logger;
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<WorkoutAnalysisResponseDto>> AnalyzeWorkouts(
        [FromBody] WorkoutAnalysisRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Analyzing workouts for analysis type: {AnalysisType}", request.AnalysisType);
            WorkoutAnalysisResponseDto result = await this.workoutAnalysisService.AnalyzeWorkoutsAsync(request, cancellationToken);
            return this.Ok(result);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error analyzing workouts");
            return this.StatusCode(500, "An error occurred while analyzing workouts");
        }
    }

    // Performance Trends Endpoint
    [HttpGet("performance-trends/{athleteId}")]
    public async Task<ActionResult<WorkoutAnalysisResponseDto>> AnalyzePerformanceTrends(
        int athleteId,
        CancellationToken cancellationToken,
        [FromQuery] string timeFrame = "month")
    {
        try
        {
            this.logger.LogInformation(
                "Analyzing performance trends for athlete: {AthleteId}, timeFrame: {TimeFrame}",
                athleteId,
                timeFrame);

            // Build request for performance trends
            WorkoutAnalysisRequestDto request = new WorkoutAnalysisRequestDto
            {
                AnalysisType = "Trends",
                RecentWorkouts = this.GetDemoWorkouts(athleteId, timeFrame), // TODO: replace with real data
                AthleteProfile = this.GetDemoAthleteProfile(athleteId), // TODO: replace with real data
                AdditionalContext = new Dictionary<string, object>
                {
                    { "timeFrame", timeFrame },
                    { "athleteId", athleteId },
                },
            };

            WorkoutAnalysisResponseDto result = await this.workoutAnalysisService.AnalyzeWorkoutsAsync(request, cancellationToken);
            return this.Ok(result);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error analyzing performance trends for athlete {AthleteId}", athleteId);
            return this.StatusCode(500, "An error occurred while analyzing performance trends");
        }
    }

    // Training Recommendations Endpoint
    [HttpGet("recommendations/{athleteId}")]
    public async Task<ActionResult<WorkoutAnalysisResponseDto>> GetTrainingRecommendations(
        int athleteId, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Getting training recommendations for athlete: {AthleteId}", athleteId);

            WorkoutAnalysisRequestDto request = new WorkoutAnalysisRequestDto
            {
                AnalysisType = "Recommendations",
                RecentWorkouts = this.GetDemoWorkouts(athleteId, "week"),
                AthleteProfile = this.GetDemoAthleteProfile(athleteId),
                AdditionalContext = new Dictionary<string, object>
                {
                    { "focus", "training_optimization" },
                    { "athleteId", athleteId },
                },
            };

            WorkoutAnalysisResponseDto result = await this.workoutAnalysisService.AnalyzeWorkoutsAsync(request, cancellationToken);
            return this.Ok(result);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error getting training recommendations for athlete {AthleteId}", athleteId);
            return this.StatusCode(500, "An error occurred while getting training recommendations");
        }
    }

    // Health Metrics Analysis Endpoint
    [HttpPost("health-analysis")]
    public async Task<ActionResult<WorkoutAnalysisResponseDto>> AnalyzeHealthMetrics(
        [FromBody] HealthAnalysisRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Analyzing health metrics for athlete: {AthleteId}", request.AthleteId);

            WorkoutAnalysisRequestDto analysisRequest = new WorkoutAnalysisRequestDto
            {
                AnalysisType = "Health",
                RecentWorkouts = request.RecentWorkouts,
                AthleteProfile = this.GetDemoAthleteProfile(request.AthleteId),
                AdditionalContext = new Dictionary<string, object>
                {
                    { "focus", "injury_prevention" },
                    { "health_analysis", true },
                    { "athleteId", request.AthleteId },
                },
            };

            WorkoutAnalysisResponseDto result = await this.workoutAnalysisService.AnalyzeWorkoutsAsync(analysisRequest, cancellationToken);
            return this.Ok(result);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error analyzing health metrics for athlete {AthleteId}", request.AthleteId);
            return this.StatusCode(500, "An error occurred while analyzing health metrics");
        }
    }

    // Health check for workout analysis service
    [HttpGet("health")]
    public async Task<ActionResult> HealthCheck(CancellationToken cancellationToken)
    {
        try
        {
            // Simple test request
            WorkoutAnalysisRequestDto testRequest = new WorkoutAnalysisRequestDto
            {
                AnalysisType = "Health Check",
                RecentWorkouts = new List<WorkoutDataDto>
                {
                    new WorkoutDataDto
                    {
                        Date = DateTime.Now.AddDays(-1),
                        ActivityType = "Run",
                        Distance = 5.0,
                        Duration = 1800,
                        Calories = 350,
                    },
                },
                AthleteProfile = new AthleteProfileDto
                {
                    Name = "Test User",
                    FitnessLevel = "Intermediate",
                    PrimaryGoal = "Health Check",
                },
            };

            WorkoutAnalysisResponseDto result = await this.workoutAnalysisService.AnalyzeWorkoutsAsync(testRequest, cancellationToken);

            return this.Ok(new
            {
                status = "healthy",
                message = "Workout analysis service is responding",
                timestamp = DateTime.UtcNow,
                analysisGenerated = !string.IsNullOrEmpty(result.Analysis),
            });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Workout analysis health check failed");
            return this.StatusCode(503, new
            {
                status = "unhealthy",
                message = ex.Message,
                timestamp = DateTime.UtcNow,
            });
        }
    }

    private List<WorkoutDataDto> GetDemoWorkouts(int athleteId, string timeFrame)
        => DemoDataProvider.GetDemoWorkouts();

    private AthleteProfileDto GetDemoAthleteProfile(int athleteId)
        => DemoDataProvider.GetDemoAthleteProfile(athleteId);
}


