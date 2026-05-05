namespace SportMatrix.AIAssistant.UI.API.Services;

using Grpc.Core;
using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.AIAssistant.Extensions;
using SportMatrix.AIAssistant.Infrastructure.Providers;

public class WorkoutAnalysisGrpcService : Sportmatrix.WorkoutService.WorkoutServiceBase
{
    private readonly IWorkoutAnalysisService workoutAnalysisService;
    private readonly ILogger<WorkoutAnalysisGrpcService> logger;

    public WorkoutAnalysisGrpcService(
        IWorkoutAnalysisService workoutAnalysisService,
        ILogger<WorkoutAnalysisGrpcService> logger)
    {
        this.workoutAnalysisService = workoutAnalysisService;
        this.logger = logger;
    }

    public override async Task<Sportmatrix.WorkoutAnalysisResponse> GetWorkoutAnalysis(
        Sportmatrix.WorkoutAnalysisRequest request,
        ServerCallContext context)
    {
        try
        {
            this.logger.LogInformation(
                "gRPC: Received workout analysis request for {WorkoutCount} workouts, type: {AnalysisType}",
                request.RecentWorkouts.Count, request.AnalysisType);

            // Convert gRPC Request to Application DTO
            WorkoutAnalysisRequestDto analysisRequest = request.ToWorkoutAnalysisRequestDto();

            WorkoutAnalysisResponseDto response = await this.workoutAnalysisService.AnalyzeWorkoutsAsync(
                analysisRequest, context.CancellationToken);

            Sportmatrix.WorkoutAnalysisResponse grpcResponse = this.ToGrpcResponse(response);

            this.logger.LogInformation("gRPC: Successfully generated workout analysis response");
            return grpcResponse;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error analyzing workouts");
            throw new RpcException(new Status(StatusCode.Internal, $"Failed to analyze workouts: {ex.Message}"));
        }
    }

    // Performance Trends Service
    public override async Task<Sportmatrix.WorkoutAnalysisResponse> GetPerformanceTrends(
        Sportmatrix.PerformanceTrendsRequest request,
        ServerCallContext context)
    {
        try
        {
            this.logger.LogInformation(
                "gRPC: Received performance trends request for athlete: {AthleteId}",
                request.AthleteId);

            WorkoutAnalysisRequestDto analysisRequest = new WorkoutAnalysisRequestDto
            {
                AnalysisType = "Trends",
                RecentWorkouts = this.GetDemoWorkouts(request.AthleteId, request.TimeFrame),
                AthleteProfile = this.GetDemoAthleteProfile(request.AthleteId),
                AdditionalContext = new Dictionary<string, object>
                {
                    { "timeFrame", request.TimeFrame },
                    { "athleteId", request.AthleteId },
                },
            };

            WorkoutAnalysisResponseDto response = await this.workoutAnalysisService.AnalyzeWorkoutsAsync(
                analysisRequest, context.CancellationToken);

            Sportmatrix.WorkoutAnalysisResponse grpcResponse = this.ToGrpcResponse(response, "PerformanceTrends");

            return grpcResponse;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error getting performance trends");
            throw new RpcException(new Status(StatusCode.Internal, $"Failed to get performance trends: {ex.Message}"));
        }
    }

    // Training Recommendations Service
    public override async Task<Sportmatrix.WorkoutAnalysisResponse> GetTrainingRecommendations(
        Sportmatrix.TrainingRecommendationsRequest request,
        ServerCallContext context)
    {
        try
        {
            this.logger.LogInformation(
                "gRPC: Received training recommendations request for athlete: {AthleteId}",
                request.AthleteId);

            WorkoutAnalysisRequestDto analysisRequest = new WorkoutAnalysisRequestDto
            {
                AnalysisType = "Recommendations",
                RecentWorkouts = this.GetDemoWorkouts(request.AthleteId, "week"),
                AthleteProfile = this.GetDemoAthleteProfile(request.AthleteId),
                AdditionalContext = new Dictionary<string, object>
                {
                    { "focus", "training_optimization" },
                    { "athleteId", request.AthleteId },
                },
            };

            WorkoutAnalysisResponseDto response = await this.workoutAnalysisService.AnalyzeWorkoutsAsync(
                analysisRequest, context.CancellationToken);

            Sportmatrix.WorkoutAnalysisResponse grpcResponse = this.ToGrpcResponse(response, "TrainingRecommendations");

            return grpcResponse;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error getting training recommendations");
            throw new RpcException(new Status(StatusCode.Internal, $"Failed to get training recommendations: {ex.Message}"));
        }
    }

    // Health Metrics Analysis Service
    public override async Task<Sportmatrix.WorkoutAnalysisResponse> AnalyzeHealthMetrics(
        Sportmatrix.HealthAnalysisRequest request,
        ServerCallContext context)
    {
        try
        {
            this.logger.LogInformation(
                "gRPC: Received health metrics analysis request for athlete: {AthleteId}",
                request.AthleteId);

            WorkoutAnalysisRequestDto analysisRequest = new WorkoutAnalysisRequestDto
            {
                AnalysisType = "Health",
                RecentWorkouts = request.RecentWorkouts.Select(w => new WorkoutDataDto
                {
                    Date = DateTime.Parse(w.Date),
                    ActivityType = w.ActivityType,
                    Distance = w.Distance,
                    Duration = w.Duration,
                    Calories = w.Calories,
                }).ToList(),
                AthleteProfile = this.GetDemoAthleteProfile(request.AthleteId),
                AdditionalContext = new Dictionary<string, object>
                {
                    { "focus", "injury_prevention" },
                    { "athleteId", request.AthleteId },
                },
            };

            WorkoutAnalysisResponseDto response = await this.workoutAnalysisService.AnalyzeWorkoutsAsync(
                analysisRequest, context.CancellationToken);

            Sportmatrix.WorkoutAnalysisResponse grpcResponse = this.ToGrpcResponse(response, "HealthMetrics");

            return grpcResponse;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error analyzing health metrics");
            throw new RpcException(new Status(StatusCode.Internal, $"Failed to analyze health metrics: {ex.Message}"));
        }
    }

    // Health Check Service
    public override Task<Sportmatrix.HealthCheckResponse> CheckHealth(
        Sportmatrix.HealthCheckRequest request,
        ServerCallContext context)
    {
        this.logger.LogInformation("gRPC: Health check request received");

        Sportmatrix.HealthCheckResponse response = new Sportmatrix.HealthCheckResponse
        {
            IsHealthy = true,
            Message = "Workout analysis service is healthy",
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        };

        return Task.FromResult(response);
    }

    private Sportmatrix.WorkoutAnalysisResponse ToGrpcResponse(
        WorkoutAnalysisResponseDto response, string? analysisType = null)
    {
        Sportmatrix.WorkoutAnalysisResponse grpcResponse = new Sportmatrix.WorkoutAnalysisResponse
        {
            Analysis = response.Analysis ?? string.Empty,
            GeneratedAt = response.GeneratedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            Source = response.Provider ?? "Gemini-AI",
            AnalysisType = analysisType ?? string.Empty,
        };

        if (response.KeyInsights != null)
        {
            grpcResponse.KeyInsights.AddRange(response.KeyInsights);
        }

        if (response.Recommendations != null)
        {
            grpcResponse.Recommendations.AddRange(response.Recommendations);
        }

        return grpcResponse;
    }

    private List<WorkoutDataDto> GetDemoWorkouts(int athleteId, string timeFrame)
        => DemoDataProvider.GetDemoWorkouts();

    private AthleteProfileDto GetDemoAthleteProfile(int athleteId)
        => DemoDataProvider.GetDemoAthleteProfile(athleteId);
}
