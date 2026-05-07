namespace SportMatrix.AIAssistant.UI.API.Controllers;

using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.AIAssistant.Extensions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("grpc-json")]
public class GrpcJsonController : ControllerBase
{
    private readonly IWorkoutAnalysisService workoutAnalysisService;
    private readonly ILogger<GrpcJsonController> logger;

    public GrpcJsonController(
        IWorkoutAnalysisService workoutAnalysisService,
        ILogger<GrpcJsonController> logger)
    {
        this.workoutAnalysisService = workoutAnalysisService;
        this.logger = logger;
    }


    /// <summary>
    /// Health check for gRPC-JSON Bridge
    /// </summary>
    [HttpGet("health")]
    public Task<ActionResult> HealthCheckAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult>(this.Ok(new
        {
            status = "healthy",
            service = "gRPC-JSON Bridge",
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            availableEndpoints = new[]
            {
                "POST /grpc-json/WorkoutService/GetWorkoutAnalysisAsync",
                "POST /grpc-json/WorkoutService/AnalyzeWorkoutsAsync",
                "POST /grpc-json/WorkoutService/GetPerformanceTrendsAsync",
                "POST /grpc-json/WorkoutService/GetTrainingRecommendationsAsync",
                "POST /grpc-json/WorkoutService/AnalyzeHealthMetricsAsync",
                "GET /grpc-json/health",
            },
        }));
    }

    /// <summary>
    /// gRPC-JSON Bridge for WorkoutService.GetWorkoutAnalysis
    /// </summary>
    [HttpPost("WorkoutService/GetWorkoutAnalysisAsync")]
    public async Task<ActionResult> GetWorkoutAnalysisAsync([FromBody] GrpcJsonWorkoutAnalysisRequestDto request, CancellationToken cancellationToken)
    {
        this.logger.LogInformation(
            "gRPC-JSON: Received workout analysis request for {WorkoutCount} workouts",
            request.RecentWorkouts?.Length ?? 0);

        // Convert JSON to Application DTO
        WorkoutAnalysisRequestDto workoutAnalysisRequest = request.ToWorkoutAnalysisRequestDto();

        // Call the service (uses Gemini as default)
        WorkoutAnalysisResponseDto response = await this.workoutAnalysisService.AnalyzeWorkoutsAsync(workoutAnalysisRequest, cancellationToken);

        // Convert Response to gRPC-JSON format
        var grpcJsonResponse = new
        {
            analysis = response.Analysis ?? string.Empty,
            keyInsights = response.KeyInsights ?? new List<string>(),
            recommendations = response.Recommendations ?? new List<string>(),
            generatedAt = response.GeneratedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            source = "gRPC-JSON",
        };

        this.logger.LogInformation("gRPC-JSON: Successfully generated workout analysis response");
        return this.Ok(grpcJsonResponse);
    }

    /// <summary>
    /// Placeholder - can be expanded when service is available
    /// </summary>
    [HttpPost("WorkoutService/GetPerformanceTrendsAsync")]
    public Task<ActionResult> GetPerformanceTrendsAsync([FromBody] GrpcJsonPerformanceTrendsRequestDto request, CancellationToken cancellationToken)
    {
        // Since no corresponding service is available, return a mock response
        var mockResponse = new
        {
            analysis = "Performance trends analysis is not yet available via gRPC-JSON. Use the REST API endpoint instead.",
            keyInsights = new[] { "Feature coming soon" },
            recommendations = new[] { "Use REST API: GET /api/WorkoutAnalysis/performance-trends/{athleteId}" },
            generatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            source = "gRPC-JSON-Mock",
        };

        return Task.FromResult<ActionResult>(this.Ok(mockResponse));
    }

    /// <summary>
    /// Placeholder - can be expanded when service is available
    /// </summary>
    [HttpPost("WorkoutService/GetTrainingRecommendationsAsync")]
    public Task<ActionResult> GetTrainingRecommendationsAsync([FromBody] GrpcJsonTrainingRecommendationsRequestDto request, CancellationToken cancellationToken)
    {
        // Since no corresponding service is available, return a mock response
        var mockResponse = new
        {
            analysis = "Training recommendations are not yet available via gRPC-JSON. Use the REST API endpoint instead.",
            keyInsights = new[] { "Feature coming soon" },
            recommendations = new[] { "Use REST API: GET /api/WorkoutAnalysis/recommendations/{athleteId}" },
            generatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            source = "gRPC-JSON-Mock",
        };

        return Task.FromResult<ActionResult>(this.Ok(mockResponse));
    }

    /// <summary>
    /// Placeholder - can be expanded when service is available
    /// </summary>
    [HttpPost("WorkoutService/AnalyzeHealthMetricsAsync")]
    public Task<ActionResult> AnalyzeHealthMetricsAsync([FromBody] GrpcJsonHealthMetricsRequestDto request, CancellationToken cancellationToken)
    {
        // Since no corresponding service is available, return a mock response
        var mockResponse = new
        {
            analysis = "Health metrics analysis is not yet available via gRPC-JSON. Use the REST API endpoint instead.",
            keyInsights = new[] { "Feature coming soon" },
            recommendations = new[] { "Use REST API: POST /api/WorkoutAnalysis/health-analysis" },
            generatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            source = "gRPC-JSON-Mock",
        };

        return Task.FromResult<ActionResult>(this.Ok(mockResponse));
    }
}
