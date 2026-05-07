namespace SportMatrix.AIAssistant.UI.API.Controllers;

using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WorkoutAnalysisController : ControllerBase
{
    private readonly IWorkoutAnalysisService workoutAnalysisService;
    private readonly IWorkoutDataProvider workoutDataProvider;
    private readonly ILogger<WorkoutAnalysisController> logger;

    public WorkoutAnalysisController(
        IWorkoutAnalysisService workoutAnalysisService,
        IWorkoutDataProvider workoutDataProvider,
        ILogger<WorkoutAnalysisController> logger)
    {
        this.workoutAnalysisService = workoutAnalysisService;
        this.workoutDataProvider = workoutDataProvider;
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

            List<WorkoutDataDto> recentWorkouts = await this.workoutDataProvider.GetRecentWorkoutsAsync(
                athleteId,
                TimeSpan.FromDays(14),
                cancellationToken);

            WorkoutAnalysisRequestDto request = new WorkoutAnalysisRequestDto
            {
                AnalysisType = AnalysisType.Trends,
                RecentWorkouts = recentWorkouts,
                AthleteProfile = this.workoutDataProvider.BuildAthleteProfile(athleteId, recentWorkouts),
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

            List<WorkoutDataDto> recentWorkouts = await this.workoutDataProvider.GetRecentWorkoutsAsync(
                athleteId,
                TimeSpan.FromDays(14),
                cancellationToken);

            WorkoutAnalysisRequestDto request = new WorkoutAnalysisRequestDto
            {
                AnalysisType = AnalysisType.Recommendations,
                RecentWorkouts = recentWorkouts,
                AthleteProfile = this.workoutDataProvider.BuildAthleteProfile(athleteId, recentWorkouts),
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

    // Next-Day Recommendation Endpoint
    [HttpGet("next-day-recommendation/{athleteId}")]
    public async Task<ActionResult<WorkoutAnalysisResponseDto>> GetNextDayRecommendation(
        int athleteId, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Getting next-day recommendation for athlete: {AthleteId}", athleteId);

            List<WorkoutDataDto> latestWorkout = await this.workoutDataProvider.GetLatestWorkoutsAsync(
                athleteId,
                1,
                cancellationToken);

            WorkoutAnalysisRequestDto request = new WorkoutAnalysisRequestDto
            {
                AnalysisType = AnalysisType.NextDay,
                RecentWorkouts = latestWorkout,
                AthleteProfile = this.workoutDataProvider.BuildAthleteProfile(athleteId, latestWorkout),
                AdditionalContext = new Dictionary<string, object>
                {
                    { "focus", "recovery_planning" },
                    { "athleteId", athleteId },
                },
            };

            WorkoutAnalysisResponseDto result = await this.workoutAnalysisService.AnalyzeWorkoutsAsync(request, cancellationToken);
            return this.Ok(result);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error getting next-day recommendation for athlete {AthleteId}", athleteId);
            return this.StatusCode(500, "An error occurred while getting next-day recommendation");
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

            List<WorkoutDataDto> recentWorkouts = request.RecentWorkouts.Count > 0
                ? request.RecentWorkouts
                : await this.workoutDataProvider.GetRecentWorkoutsAsync(
                    request.AthleteId,
                    TimeSpan.FromDays(14),
                    cancellationToken);

            WorkoutAnalysisRequestDto analysisRequest = new WorkoutAnalysisRequestDto
            {
                AnalysisType = AnalysisType.Health,
                RecentWorkouts = recentWorkouts,
                AthleteProfile = this.workoutDataProvider.BuildAthleteProfile(request.AthleteId, recentWorkouts),
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
                AnalysisType = AnalysisType.Performance,
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

}


